using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Stream.UserStream;

public class UserStream
{
    public Guid UserId { get; set; }
    public Guid StreamId { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    public virtual UserEntity User { get; set; }
    public virtual StreamEntity Stream { get; set; }

    private UserStream() { }

    private UserStream(
        Guid userId, 
        Guid streamId, 
        DateTime joinedAt)
    {
        UserId = userId;
        StreamId = streamId;
        JoinedAt = joinedAt;
    }

    public static UserStream Create(Guid userId, Guid streamId)
    {
        return new UserStream(
            userId,
            streamId,
            DateTime.UtcNow);
    }
}