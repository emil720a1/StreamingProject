using Microsoft.EntityFrameworkCore;
using StreamingProject.Application;
using StreamingProject.Domain;

namespace StreamingProject.Repository.Repositories;

public class SubscriptionRepositories : ISubscriptionRepository
{
    private readonly StreamingDbContext _dbContext;

    public SubscriptionRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<SubscriptionEntity> SubscribeAsync(SubscriptionEntity subscription)
    {
        await _dbContext.Subscriptions.AddAsync(subscription);
        await _dbContext.SaveChangesAsync();
            
        return subscription;
    }

    public async Task<int> UnsubscribeAsync(Guid FollowerId, Guid FollowedId)
    {
        if (FollowedId == Guid.Empty || FollowerId == Guid.Empty) return 0;

       int count =  await _dbContext.Subscriptions
            .Where(a => a.FollowedId == FollowedId && a.FollowerId == FollowerId)
            .ExecuteDeleteAsync();
       
       return count;
    }

    public async Task<List<SubscriptionEntity>> GetSubscriptionsAsync(Guid FollowerId)
    {
        return await _dbContext.Subscriptions.Where(a => a.FollowerId == FollowerId).ToListAsync();
    }
}