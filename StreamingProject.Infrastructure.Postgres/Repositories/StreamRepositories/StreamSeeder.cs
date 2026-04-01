namespace StreamingProject.Repository.Repositories.StreamRepositories;

public class StreamSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public StreamSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedAsync(StreamingDbContext context)
    {
        throw new NotImplementedException();
    }
}