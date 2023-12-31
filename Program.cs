using chat_server.AppConfigurationModule;
using chat_server.Base;

namespace chat_server;

internal static class Program
{
    private const string UrlBase = "https://localhost:8080";

    private static async Task Main(string[] args)
    {
        Console.Title = "Server";

        var storageConfigurator = new SqliteStorageConfigurator();
        var authConfigurator = new JwtTokenAuthConfigurator(new Utf8Encoder());

        var appConfigurator = new AppConfigurator(UrlBase, storageConfigurator, authConfigurator);
        await appConfigurator.Configure(args);
    }
}