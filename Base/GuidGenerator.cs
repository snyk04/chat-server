using chat_server.Controllers.Auth.Interfaces;

namespace chat_server.Base;

public class GuidGenerator : IGuidGenerator
{
    public Guid GetNewGuid()
    {
        return Guid.NewGuid();
    }
}