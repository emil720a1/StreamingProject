using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Subscription;

public class SubscriptionEntity
{
    public Guid Id { get; private set; }
    
    public Guid FollowedId { get; private set; }
    public Guid FollowerId { get; private set; }
    
    public UserEntity Followed { get; private set; }
    
    public UserEntity Follower { get; private set; }
    
    public DateTime SubscriptionAt { get; private set; }


    private SubscriptionEntity()
    {
        
    }
    private SubscriptionEntity(
        Guid id,
        Guid followedId,
        Guid followerId,
        DateTime subscriptionAt)
    {
        Id = id;
        FollowedId = followedId;
        FollowerId = followerId;
        SubscriptionAt = subscriptionAt;
    }

    public static SubscriptionEntity Create(Guid followerId, Guid followedId)
    {
        if (followerId == followedId)
            throw new InvalidOperationException("User cannot follow themselves.");
        
        return new SubscriptionEntity(
            Guid.NewGuid(),
            followedId,
            followerId,
            DateTime.UtcNow
            );
    }
}