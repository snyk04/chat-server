using chat_server.Base;

namespace chat_server.Controllers.Chat.Interfaces;

public interface IWebSocketManager
{
    Task ListenForMessages(HttpContext context, ReceiveMessageHandler onMessageReceived);
    Task SendStringToAllUsers(string message);
}