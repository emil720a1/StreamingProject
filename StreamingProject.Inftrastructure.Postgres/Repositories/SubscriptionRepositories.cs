using StreamingProject.Application;

namespace StreamingProject.Repository.Repositories;

public class SubscriptionRepositories : ISubscriptionRepository
{
    private readonly StreamingDbContext _dbContext;

    public SubscriptionRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

}