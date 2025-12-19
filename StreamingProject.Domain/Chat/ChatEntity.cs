using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Chat;

public class ChatEntity
{

    private ChatEntity(
        Guid id, 
        Guid streamId, 
        Guid userId,
        string message, 
        DateTime sentTime)
    {
        Id = id;
        StreamId = streamId;
        UserId = userId;
        Message = message;
        SentTime = sentTime;
    }


    private ChatEntity()
    {
    }

    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }
    public StreamEntity Stream { get; set; }
    
    public string Message { get; set; }
    
    public DateTime SentTime { get; set; }
    
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }

    public static ChatEntity Create(Guid streamId, Guid userId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or empty");
        }

        var chat = new ChatEntity(
            Guid.NewGuid(),
            streamId,
            userId,
            message,
            DateTime.UtcNow);
        
        return chat;
    }
}