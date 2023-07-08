using System.Security.Claims;

namespace chat_server.WebSocketModule.Interfaces;

public interface ITokenManager
{
    ClaimsPrincipal GetPrincipalFromToken(string token, string issuerSigningKey);
}