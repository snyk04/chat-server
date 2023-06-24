using Newtonsoft.Json;
using Server.Models;

namespace Server;

public class ChatController
{
    private readonly WebSocketManager webSocketManager;

    public ChatController(WebSocketManager webSocketManager)
    {
        this.webSocketManager = webSocketManager;
    }

    /// <summary>
    /// Processes all new chat messages and sends them to all users that are connected
    /// </summary>
    /// <param name="context">HttpContext</param>
    public async Task Chat(HttpContext context)
    {
        await webSocketManager.ListenForMessages(context, SendTextMessageToAllUsers);
    }

    private async void SendTextMessageToAllUsers(string messageText, string authorUsername)
    {
        var message = new TextMessage(messageText, authorUsername);
        var messageJson = JsonConvert.SerializeObject(message);
        await webSocketManager.SendStringToAllUsers(messageJson);
    }
}