namespace StreamingProject.Repository.Repositories.ChatRepositories;

public class ChatSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public ChatSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    
    public async Task SeedAsync(StreamingDbContext context)
    {
        throw new NotImplementedException();
    }
}