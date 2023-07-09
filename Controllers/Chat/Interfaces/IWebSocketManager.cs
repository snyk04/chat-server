namespace chat_server.Controllers.Chat.Interfaces;

public interface IWebSocketManager
{
    Task ListenForMessages(HttpContext context, string authorUsername, ReceiveMessageHandler onMessageReceived);
    Task SendStringToAllUsers(string message);
}