using StreamingProject.Domain.Chat;
using StreamingProject.Domain.User;
using StreamingProject.Domain.Video;

namespace StreamingProject.Domain.Stream;

public class StreamEntity
{


    private StreamEntity()
    {
        ChatMessages = new List<ChatEntity>();
        Likes = new List<StreamLikeEntity>();
    }

    private StreamEntity(
        Guid id, 
        Guid userId, 
        Guid chatId, 
        DateTime startTime, 
        int likeCount) : this()
    {
        Id = id;
        UserId = userId;
        ChatId = chatId;
        StartTime = startTime;
        LikeCount = likeCount;

    }
    public Guid Id { get; set; }
    
    public string StreamKey { get; set; }
    public Guid ChatId { get; set; }
    
    public Guid UserId { get; set; }
    
    public VideoEntity VideoEntity { get; set; }
    
    public UserEntity User { get; set; }
    
    public ICollection<ChatEntity> ChatMessages { get; set; }
    
    public ICollection<StreamLikeEntity> Likes { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public int LikeCount { get; set; }



    public static StreamEntity Create(Guid userId)
    {
        return new StreamEntity
        (
            Guid.NewGuid(),
            userId,
            Guid.NewGuid(),
            DateTime.UtcNow,
            0
        );
    }
    
}