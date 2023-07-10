using System.Security.Claims;
using chat_server.AuthModule.Interfaces;
using chat_server.Controllers.Chat.Interfaces;

namespace chat_server.AuthModule;

public class JwtTokenUsernameProvider : IUsernameProvider
{
    private readonly ITokenManager tokenManager;
    private readonly IConfiguration configuration;

    public JwtTokenUsernameProvider(ITokenManager tokenManager, IConfiguration configuration)
    {
        this.tokenManager = tokenManager;
        this.configuration = configuration;
    }

    public string GetUserName(HttpContext httpContext)
    {
        var token = GetTokenFromHttpContext(httpContext);
        var principal = GetPrincipalFromToken(token);
        return principal.Identity?.Name;
    }

    private string GetTokenFromHttpContext(HttpContext httpContext)
    {
        return httpContext.Request.Query["token"];
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        return tokenManager.GetPrincipalFromToken(token, configuration["JWT:secret"]);
    }
}