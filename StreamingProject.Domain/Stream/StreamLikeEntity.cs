namespace StreamingProject.Domain.Stream;

public class StreamLikeEntity
{
    public Guid Id { get; private set; }

    public Guid StreamId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime? LikeTime { get; private set; }

    private StreamLikeEntity()
    {
    }

    private StreamLikeEntity(
        Guid id,
        Guid streamId,
        Guid userId,
        DateTime? likeTime)
    {
        Id = id;
        StreamId = streamId;
        UserId = userId;
        LikeTime = likeTime;
    }

    public static StreamLikeEntity Create(Guid streamId, Guid userId)
    {
        return new StreamLikeEntity(
            Guid.NewGuid(),
            streamId,
            userId,
            DateTime.UtcNow
        );
    }

}