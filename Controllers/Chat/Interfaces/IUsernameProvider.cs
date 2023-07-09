namespace chat_server.Controllers.Chat.Interfaces;

public interface IUsernameProvider
{
    string GetUserName(HttpContext httpContext);
}