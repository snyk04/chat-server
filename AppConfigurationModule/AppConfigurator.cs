using chat_server.AppConfigurationModule.Interfaces;
using chat_server.AuthModule;
using chat_server.AuthModule.Interfaces;
using chat_server.Base;
using chat_server.Controllers.Auth.Interfaces;
using chat_server.Controllers.Chat.Interfaces;
using chat_server.WebSocketModule.Interfaces;
using IUtf8Encoder = chat_server.AppConfigurationModule.Interfaces.IUtf8Encoder;
using WebSocketManager = chat_server.WebSocketModule.WebSocketManager;

namespace chat_server.AppConfigurationModule;

public class AppConfigurator
{
    private readonly string urlBase;
    private readonly IStorageConfigurator storageConfigurator;
    private readonly IAuthConfigurator authConfigurator;

    public AppConfigurator(string urlBase, IStorageConfigurator storageConfigurator, IAuthConfigurator authConfigurator)
    {
        this.urlBase = urlBase;
        this.storageConfigurator = storageConfigurator;
        this.authConfigurator = authConfigurator;
    }

    public async Task Configure(string[] args)
    {
        var builder = CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        ConfigureApp(app);
        await app.RunAsync();
    }

    private WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls(urlBase);
        return builder;
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        storageConfigurator.Configure(services, configuration);
        authConfigurator.Configure(services, configuration);
        ConfigureDependencies(services);
        ConfigureCors(services);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    private void ConfigureDependencies(IServiceCollection services)
    {
        services.AddScoped<IAsciiDecoder, AsciiDecoder>();
        services.AddScoped<IAsciiEncoder, AsciiEncoder>();
        services.AddScoped<IGuidGenerator, GuidGenerator>();
        services.AddScoped<IJsonConverter, NewtonsoftJsonConverter>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<IUsernameProvider, JwtTokenUsernameProvider>();
        services.AddScoped<IUtf8Encoder, Utf8Encoder>();
        services.AddScoped<IWebSocketManager, WebSocketManager>();
    }

    private void ConfigureCors(IServiceCollection services)
    {
        // TODO : Think about CORS restrictions.
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });
    }

    private void ConfigureApp(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseWebSockets();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCors();
    }
}