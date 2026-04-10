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
        string streamKey, 
        Guid chatId) : this()
    {
        Id = id;
        UserId = userId;
        StreamKey = streamKey;
        ChatId = chatId;
    }
    
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public Guid Id { get; private set; }
    
    public string StreamKey { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ChatId { get; private set; }
    
    
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public UserEntity User { get; private set; }
    public VideoEntity VideoEntity { get; private set; }

    private readonly List<UserStream.UserStream> _participants = new();
    public IReadOnlyList<UserStream.UserStream> Participants => _participants;
    
    public ICollection<ChatEntity> ChatMessages { get; private set; }
    
    public ICollection<StreamLikeEntity> Likes { get; private set; }
    
    public static StreamEntity Create(Guid userId)
    {
        
        var key = $"sk_{Guid.NewGuid().ToString("N").Substring(0, 12)}";
        return new StreamEntity
        (
            Guid.NewGuid(),
            userId,
            key,
           Guid.NewGuid()
        );
    }

    public void UpdateMetadata(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Title cannot be empty.");
        
        Title = title;
        Description = description;
    }

    public void StartStream()
    {
        StartTime = DateTime.UtcNow;
        EndTime = null;
    }

    public void EndStream()
    {
        EndTime = DateTime.UtcNow;
    }
}