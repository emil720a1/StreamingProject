namespace StreamingProject.Repository.Repositories.PermissionRepositories;

public class PermissionSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public PermissionSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public Task SeedAsync()
    {
        throw new NotImplementedException();
    }
}