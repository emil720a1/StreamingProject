using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StreamingProject.Repository;

public class DbInitializer(
    StreamingDbContext streamingDbContext,
    IEnumerable<ISeeder> seeders,
    ILogger<DbInitializer> logger) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Applying pending database migrations");
            await streamingDbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Database migrations completed successfully");

            foreach (var seeder in seeders)
            {
                logger.LogInformation(
                    "Running database seeder {SeederName}",
                    seeder.GetType().Name);

                await seeder.SeedAsync(streamingDbContext);
            }

            logger.LogInformation("Database seeders completed successfully");
        }
        catch (Exception exception)
        {
            logger.LogCritical(
                exception,
                "Database initialization failed. The application will not start");

            throw;
        }
    }
}
