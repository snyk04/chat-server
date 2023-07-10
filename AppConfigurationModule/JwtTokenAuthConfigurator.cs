using chat_server.AppConfigurationModule.Interfaces;
using chat_server.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace chat_server.AppConfigurationModule;

public class JwtTokenAuthConfigurator : IAuthConfigurator
{
    private readonly IUtf8Encoder utf8Encoder;

    public JwtTokenAuthConfigurator(IUtf8Encoder utf8Encoder)
    {
        this.utf8Encoder = utf8Encoder;
    }

    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        ConfigureAuthTokens(services, configuration);
        ConfigurePasswordSeverity(services);
    }

    private void ConfigureAuthTokens(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(ConfigureAuthentication).AddJwtBearer(options =>
        {
            ConfigureJwtBearer(options, configuration);
        });
    }

    private void ConfigureAuthentication(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    private void ConfigureJwtBearer(JwtBearerOptions options, IConfiguration configuration)
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = GetTokenValidationParameters(configuration);
    }

    private TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(utf8Encoder.GetBytes(configuration["JWT:Secret"]))
        };
    }

    private void ConfigurePasswordSeverity(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
        });
    }
}