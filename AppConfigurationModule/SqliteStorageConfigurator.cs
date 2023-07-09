using chat_server.AppConfigurationModule.Interfaces;
using chat_server.Storage;
using Microsoft.EntityFrameworkCore;

namespace chat_server.AppConfigurationModule;

public class SqliteStorageConfigurator : IStorageConfigurator
{
    private const string ConnectionString = "Data Source=database.db";

    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(ConnectionString));
    }
}