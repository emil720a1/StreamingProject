namespace StreamingProject.Repository.Repositories.StreamRepositories;

public class StreamSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public StreamSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public Task SeedAsync()
    {
        throw new NotImplementedException();
    }
}