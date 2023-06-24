using chat_server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace chat_server;

public class ChatController : Controller
{
    private readonly WebSocketManager webSocketManager;

    public ChatController(WebSocketManager webSocketManager)
    {
        this.webSocketManager = webSocketManager;
    }

    /// <summary>
    /// Processes all new chat messages and sends them to all users that are connected
    /// </summary>
    [Route("/Chat")]
    public async Task Chat()
    {
        await webSocketManager.ListenForMessages(HttpContext, SendTextMessageToAllUsers);
    }

    private async void SendTextMessageToAllUsers(string messageText, string authorUsername)
    {
        var message = new TextMessage(messageText, authorUsername);
        var messageJson = JsonConvert.SerializeObject(message);
        await webSocketManager.SendStringToAllUsers(messageJson);
    }
}