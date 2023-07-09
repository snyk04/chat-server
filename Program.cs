using System.Text;
using chat_server.Base;
using chat_server.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebSocketManager = chat_server.Base.WebSocketManager;

namespace chat_server;

internal static class Program
{
    private const string ConnectionString = "Data Source=database.db";
    private const string UrlBase = "https://localhost:8080";
    
    public static async Task Main(string[] args)
    {
        Console.Title = "Server";

        var builder = CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = CreateApp(builder);
        ConfigureApp(app);
        await app.RunAsync();
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls(UrlBase);
        return builder;
    }

    private static WebApplication CreateApp(WebApplicationBuilder builder)
    {
        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWebSocketManager, WebSocketManager>();
        services.AddScoped<IJsonConverter, NewtonsoftJsonConverter>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<IAsciiEncoder, AsciiEncoder>();
        services.AddScoped<IUtf8Encoder, Utf8Encoder>();
        services.AddScoped<IGuidGenerator, GuidGenerator>();
        
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(ConnectionString));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,

                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // TODO : Think about it
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }
    
    private static void ConfigureApp(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseWebSockets();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCors();
    }
}