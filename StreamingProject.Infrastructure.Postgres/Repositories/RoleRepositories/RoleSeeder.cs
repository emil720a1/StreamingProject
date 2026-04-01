namespace StreamingProject.Repository.Repositories.RoleRepositories;

public class RoleSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public RoleSeeder(StreamingDbContext context)
    {
        _context = context;
    }

    public Task SeedAsync(StreamingDbContext context)
    {
        throw new NotImplementedException();
    }
}