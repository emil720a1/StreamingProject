using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User;

namespace StreamingProject.Domain.Chat;

public class ChatEntity
{

    private ChatEntity() { }
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
    public Guid Id { get; private set; }
    
    public Guid StreamId { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime SentTime { get; private set; }
    public StreamEntity Stream { get; private set; }
    public UserEntity User { get; private set; }

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