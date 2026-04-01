namespace StreamingProject.Repository.Repositories.VideoRepositories;


public class VideoSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public VideoSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedAsync(StreamingDbContext context)
    {
        throw new NotImplementedException();
    }
}