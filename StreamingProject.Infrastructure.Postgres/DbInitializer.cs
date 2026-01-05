using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace StreamingProject.Repository.StreamingProject.Infrastructure;

public class DbInitializer(StreamingDbContext streamingDbContext) : IAsyncInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await streamingDbContext.Database.MigrateAsync(cancellationToken);
    }
}