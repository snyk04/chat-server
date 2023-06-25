using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace chat_server.Base;

public class WebSocketManager
{
    private readonly IConfiguration configuration;
    private readonly TokenManager tokenManager;

    private readonly ConcurrentDictionary<string, WebSocket> webSocketsByUsernames;

    public WebSocketManager(IConfiguration configuration, TokenManager tokenManager)
    {
        this.configuration = configuration;
        this.tokenManager = tokenManager;
        webSocketsByUsernames = new ConcurrentDictionary<string, WebSocket>();
    }

    /// <summary>
    /// Listens for websockets messages. If there is new message - onMessageReceived invoked with message text and
    /// username
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="onMessageReceived">Action that invokes if there is new message on websocket. Action format is (text, username)</param>
    /// <exception cref="ArgumentException">Thrown if username in query is null</exception>
    public async Task ListenForMessages(HttpContext context, Action<string, string>? onMessageReceived)
    {
        string token = context.Request.Query["token"];
        var principal = tokenManager.GetPrincipalFromToken(token, configuration["JWT:secret"]);
        var authorUsername = principal.Identity.Name;

        if (authorUsername == null)
        {
            throw new ArgumentException("Author username can't be null!");
        }

        try
        {
            await HandleUserRequest(context, authorUsername, onMessageReceived);
        }
        catch
        {
            HandleUserDisconnected(authorUsername);
        }
    }

    private async Task HandleUserRequest(HttpContext context, string username,
        Action<string, string>? onMessageReceived)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        webSocketsByUsernames.TryAdd(username, webSocket);

        var buffer = new byte[256];
        while (true)
        {
            await HandleUserMessage(webSocket, buffer, username, onMessageReceived);
        }
    }

    private async Task HandleUserMessage(WebSocket webSocket, byte[] buffer, string username,
        Action<string, string>? onMessageReceived)
    {
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        var text = Encoding.ASCII.GetString(buffer, 0, result.Count);
        onMessageReceived?.Invoke(text, username);
    }

    private void HandleUserDisconnected(string username)
    {
        webSocketsByUsernames.TryRemove(username, out _);
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
        var messageBytes = Encoding.ASCII.GetBytes(message);
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}