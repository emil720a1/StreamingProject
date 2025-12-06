using StreamingProject.Domain;
using StreamingProject.Domain.Subscription;

namespace StreamingProject.Application.Service.Subscription.SubscriptionRepository;

public interface ISubscriptionRepository
{
    Task<SubscriptionEntity> SubscribeAsync(SubscriptionEntity subscription);
    
    Task<int> UnsubscribeAsync(Guid FollowerId, Guid FollowedId);

    Task<List<SubscriptionEntity>> GetSubscriptionsAsync(Guid FollowerId);
    
}   