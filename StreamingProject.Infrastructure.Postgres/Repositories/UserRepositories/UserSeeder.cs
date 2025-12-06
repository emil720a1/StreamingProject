namespace StreamingProject.Repository.Repositories.UserRepositories;

public class UserSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public UserSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public Task SeedAsync()
    {
        throw new NotImplementedException();
    }
}