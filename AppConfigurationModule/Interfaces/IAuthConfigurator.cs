namespace chat_server.AppConfigurationModule.Interfaces;

public interface IAuthConfigurator
{
    void Configure(IServiceCollection services, IConfiguration configuration);
}