namespace StreamingProject.Repository.Repositories.ChatRepositories;

public class ChatSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public ChatSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    
    public Task SeedAsync()
    {
        throw new NotImplementedException();
    }
}