using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Chat;

public class ChatEntity
{
    public Guid Id { get; set; }
    
    
    public Guid StreamId { get; set; }
    public StreamEntity Stream { get; set; }
    
    public string Message { get; set; }
    
    public DateTime SentTime { get; set; }
    
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    
}