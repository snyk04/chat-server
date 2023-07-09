using chat_server.Controllers.Chat.Interfaces;
using chat_server.Controllers.Chat.Models;
using Microsoft.AspNetCore.Mvc;

namespace chat_server.Controllers.Chat;

[ApiController]
[Route("[controller]/[action]")]
public class ChatController : Controller
{
    private readonly IUsernameProvider usernameProvider;
    private readonly IWebSocketManager webSocketManager;
    private readonly IJsonConverter jsonConverter;

    public ChatController(IUsernameProvider usernameProvider, IWebSocketManager webSocketManager,
        IJsonConverter jsonConverter)
    {
        this.usernameProvider = usernameProvider;
        this.webSocketManager = webSocketManager;
        this.jsonConverter = jsonConverter;
    }

    /// <summary>
    /// Processes all new chat messages and sends them to all users that are connected
    /// </summary>
    [Route("/Chat")]
    public async Task Chat()
    {
        var authorUsername = usernameProvider.GetUserName(HttpContext);

        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            await webSocketManager.ListenForMessages(HttpContext, authorUsername, SendTextMessageToAllUsers);
        }
    }

    private async void SendTextMessageToAllUsers(string messageText, string authorUsername)
    {
        var message = new TextMessage(messageText, authorUsername);
        var messageJson = jsonConverter.ConvertToJson(message);
        await webSocketManager.SendStringToAllUsers(messageJson);
    }
}