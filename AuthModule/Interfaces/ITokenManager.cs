using System.Security.Claims;

namespace chat_server.AuthModule.Interfaces;

public interface ITokenManager
{
    ClaimsPrincipal GetPrincipalFromToken(string token, string issuerSigningKey);
}