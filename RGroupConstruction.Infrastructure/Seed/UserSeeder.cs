using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Infrastructure.Seed;


public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(UserSeeder));

        var userManager = services.GetRequiredService<UserManager<User>>();
        var passwordService = services.GetRequiredService<IPasswordService>();

        await SeedAdminAsync(userManager, passwordService, logger);
    }

    private static async Task SeedAdminAsync(
        UserManager<User> userManager,
        IPasswordService passwordService,
        ILogger logger)
    {
        var adminEmail = "admin@abv.com";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
        {
            logger.Info("Admin user {AdminEmail} already exists, skipping seed", adminEmail);
            return;
        }

        var admin = new User
        {
            FirstName = "System",
            LastName = "Admin",
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            PhoneNumber = "+355691234567",
            CreatedOn = DateTime.UtcNow,
            PasswordHash = passwordService.HashPassword("Admin@123!")
        };

        var result = await userManager.CreateAsync(admin);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
            logger.Info("Seeded admin user {AdminEmail}", adminEmail);
        }
        else
            logger.Warn("Failed to seed admin user {AdminEmail}: {IdentityErrors}", adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}

