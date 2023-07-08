using System.Net.WebSockets;
using System.Text;
using chat_server.Controllers.Chat.Interfaces;

namespace chat_server.Base;

public delegate void ReceiveMessageHandler(string messageText, string username);

public class WebSocketManager : IWebSocketManager
{
    private readonly IConfiguration configuration;
    private readonly TokenManager tokenManager;

    private readonly Dictionary<string, WebSocket> webSocketsByUsernames;

    public WebSocketManager(IConfiguration configuration, TokenManager tokenManager)
    {
        this.configuration = configuration;
        this.tokenManager = tokenManager;
        webSocketsByUsernames = new Dictionary<string, WebSocket>();
    }

    /// <summary>
    /// Listens for websockets messages. If there is new message - onMessageReceived invoked with message text and
    /// username
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="onMessageReceived">Action that invokes if there is new message on websocket. Action format is (text, username)</param>
    /// <exception cref="ArgumentException">Thrown if username in query is null</exception>
    public async Task ListenForMessages(HttpContext context, ReceiveMessageHandler onMessageReceived)
    {
        string token = context.Request.Query["token"];
        var principal = tokenManager.GetPrincipalFromToken(token, configuration["JWT:secret"]);
        var authorUsername = principal.Identity?.Name;

        ArgumentNullException.ThrowIfNull(authorUsername);

        try
        {
            await HandleUserRequest(context, authorUsername, onMessageReceived);
        }
        catch
        {
            HandleUserDisconnected(authorUsername);
        }
    }

    private async Task HandleUserRequest(HttpContext context, string username, ReceiveMessageHandler onMessageReceived)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        webSocketsByUsernames.TryAdd(username, webSocket);

        // todo: cache buffer or create it with stackalloc.
        // todo: lock.
        var buffer = new byte[256];
        while (true)
        {
            await HandleUserMessage(webSocket, buffer, username, onMessageReceived);
        }
    }

    private async Task HandleUserMessage(WebSocket webSocket, byte[] buffer, string username,
        ReceiveMessageHandler onMessageReceived)
    {
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        // todo: static dependency
        var text = Encoding.ASCII.GetString(buffer, 0, result.Count);
        onMessageReceived?.Invoke(text, username);
    }

    private void HandleUserDisconnected(string username)
    {
        webSocketsByUsernames.Remove(username, out _);
        // todo: static dependency
        Console.WriteLine($"{username} disconnected");
    }

    /// <summary>
    /// Sends string message to all users that are connected through websockets
    /// </summary>
    /// <param name="message">Message text</param>
    public async Task SendStringToAllUsers(string message)
    {
        foreach (var (_, webSocket) in webSocketsByUsernames)
        {
            await SendStringToUser(webSocket, message);
        }
    }

    private async Task SendStringToUser(WebSocket webSocket, string message)
    {
        // todo: static dependency
        var messageBytes = Encoding.ASCII.GetBytes(message);
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}