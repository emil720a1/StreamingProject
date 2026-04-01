using StreamingProject.Repository;

namespace StreamProject.Web.Seeders;

public static class SeederExtensions
{
    public static async Task<WebApplication> UseSeeders(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<StreamingDbContext>();
        
        var seeders = scope.ServiceProvider.GetServices<ISeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(context);
        }
        
        return app;
    }
}