namespace StreamingProject.Repository.Repositories.SubscriptionRepositories;

public class SubscriptionSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public SubscriptionSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedAsync(StreamingDbContext context)
    {
        throw new NotImplementedException();
    }
}