using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Subscription;

public class SubscriptionEntity
{
    public Guid Id { get; set; }
    
    public Guid FollowedId { get; set; }
    
    public Guid FollowerId { get; set; }
    
    public UserEntity Followed { get; set; }
    
    public UserEntity Follower { get; set; }
    
    public DateTime SubscriptionAt { get; set; }
    
    
}