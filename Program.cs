namespace Server;

internal static class Program
{
    private const string UrlBase = "https://localhost:8080";
    
    public static async Task Main()
    {
        Console.Title = "Server";

        var app = CreateApp();
        ConfigureApp(app);
        await app.RunAsync();
    }

    private static WebApplication CreateApp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(UrlBase);
        return builder.Build();
    }
    
    private static void ConfigureApp(WebApplication app)
    {
        app.UseWebSockets();
        ConfigureRoutes(app);
    }

    private static void ConfigureRoutes(IEndpointRouteBuilder app)
    {
        var webSocketManager = new WebSocketManager();
        var chatController = new ChatController(webSocketManager);
        
        app.Map("/chat", chatController.Chat);
    }
}