using StreamingProject.Domain;

namespace StreamingProject.Application;

public interface ISubscriptionRepository
{
    Task<SubscriptionEntity> SubscribeAsync(SubscriptionEntity subscription);
    
    Task<int> UnsubscribeAsync(Guid FollowerId, Guid FollowedId);

    Task<List<SubscriptionEntity>> GetSubscriptionsAsync(Guid FollowerId);
    
}   