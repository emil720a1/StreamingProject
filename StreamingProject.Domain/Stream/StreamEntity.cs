using StreamingProject.Domain.Chat;
using StreamingProject.Domain.User;
using StreamingProject.Domain.Video;

namespace StreamingProject.Domain.Stream;

public class StreamEntity
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public Guid UserId { get; set; }
    
    public VideoEntity VideoEntity { get; set; }
    
    public UserEntity User { get; set; }
    
    public ICollection<ChatEntity> ChatMessages { get; set; }
    
    public ICollection<StreamLikeEntity> Likes { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    
    public int LikeCount { get; set; }
    
    
}