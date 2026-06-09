using RGroupConstruction.Application.Common.Constants;
using RGroupConstruction.Application.Common.Email;
using RGroupConstruction.Application.Interfaces;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Infrastructure.Presistence;
using RGroupConstruction.Infrastructure.Presistence.UnitOfWork;
using RGroupConstruction.Infrastructure.Repositories;
using RGroupConstruction.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RGroupConstruction.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.Default)!;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            ));

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        AddJwtAuthentication(services, configuration);

        services.AddHttpContextAccessor();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.Configure<EmailSettings>(configuration.GetSection(ConfigurationKeys.Sections.Email));

        services.AddSignalR();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationQueryService, NotificationQueryService>();

        return services;
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
        var secretKey = jwtSettings[ConfigurationKeys.JwtSettingKeys.SecretKey]
            ?? throw new InvalidOperationException("JWT secret key is not configured.");
        var issuer = jwtSettings[ConfigurationKeys.JwtSettingKeys.Issuer] ?? "RGroupConstruction";
        var audience = jwtSettings[ConfigurationKeys.JwtSettingKeys.Audience] ?? "RGroupConstructionUsers";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs/notifications"))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });
    }
}

