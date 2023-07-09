namespace chat_server.Controllers.Auth.Interfaces;

public interface IGuidGenerator
{
    Guid GetNewGuid();
}