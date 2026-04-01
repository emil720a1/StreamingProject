using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Subscription.SubscriptionRepository;
using StreamingProject.Domain.Subscription;

namespace StreamingProject.Repository.Repositories.SubscriptionRepositories;

public class SubscriptionRepositories : ISubscriptionRepository
{
    private readonly StreamingDbContext _dbContext;

    public SubscriptionRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<SubscriptionEntity> SubscribeAsync(SubscriptionEntity subscription, CancellationToken isAny)
    {
        await _dbContext.Subscriptions.AddAsync(subscription);
        await _dbContext.SaveChangesAsync();
            
        return subscription;
    }

    public async Task<bool> UnsubscribeAsync(Guid FollowerId, Guid FollowedId)
    {
        if (FollowedId == Guid.Empty || FollowerId == Guid.Empty) return false;

       int deletedRows =  await _dbContext.Subscriptions
            .Where(a => a.FollowedId == FollowedId && a.FollowerId == FollowerId)
            .ExecuteDeleteAsync();
       
       return deletedRows > 0;
    }

    public async Task<List<SubscriptionEntity>> GetSubscriptionsAsync(Guid FollowerId)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(s => s.Followed)
            .Where(s => s.FollowedId == FollowerId)
            .ToListAsync();
    }
}