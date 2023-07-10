using System.Net.WebSockets;
using chat_server.Controllers.Chat.Interfaces;
using chat_server.WebSocketModule.Interfaces;

namespace chat_server.WebSocketModule;

public class WebSocketManager : IWebSocketManager
{
    private readonly IAsciiEncoder asciiEncoder;
    private readonly IAsciiDecoder asciiDecoder;

    private static Dictionary<string, WebSocket> webSocketsByUsernames = new();

    public WebSocketManager(IAsciiEncoder asciiEncoder, IAsciiDecoder asciiDecoder)
    {
        this.asciiEncoder = asciiEncoder;
        this.asciiDecoder = asciiDecoder;
    }

    /// <summary>
    /// Listens for websockets messages. If there is new message - onMessageReceived invoked
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="authorUsername"></param>
    /// <param name="onMessageReceived">Action that invokes if there is new message on websocket</param>
    /// <exception cref="ArgumentException">Thrown if username in query is null</exception>
    public async Task ListenForMessages(HttpContext context, string authorUsername, ReceiveMessageHandler onMessageReceived)
    {
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
        
        while (true)
        {
            await HandleUserMessage(webSocket, username, onMessageReceived);
        }
    }

    private async Task HandleUserMessage(WebSocket webSocket, string username, ReceiveMessageHandler onMessageReceived)
    {
        // todo: cache buffer or create it with stackalloc.
        // todo: lock.
        var buffer = new byte[256];
        
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        var text = asciiDecoder.GetString(buffer, 0, result.Count);
        onMessageReceived?.Invoke(text, username);
    }

    private void HandleUserDisconnected(string username)
    {
        webSocketsByUsernames.Remove(username, out _);
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
        var messageBytes = asciiEncoder.GetBytes(message);
        await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}