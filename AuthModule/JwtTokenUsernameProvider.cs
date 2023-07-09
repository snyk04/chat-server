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
        string token = httpContext.Request.Query["token"];
        var principal = tokenManager.GetPrincipalFromToken(token, configuration["JWT:secret"]);
        return principal.Identity?.Name;
    }
}