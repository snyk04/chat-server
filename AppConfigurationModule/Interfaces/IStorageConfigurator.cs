namespace chat_server.AppConfigurationModule.Interfaces;

public interface IStorageConfigurator
{
    void Configure(IServiceCollection services, IConfiguration configuration);
}