using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Infrastructure.Seed;

public class DatabaseSeeder
{
    public static async Task SeedAllAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseSeeder));

        logger.Info("Starting database seeding");

        await RoleSeeder.SeedAsync(services);
        await UserSeeder.SeedAsync(services);

        logger.Info("Database seeding completed");
    }
}

