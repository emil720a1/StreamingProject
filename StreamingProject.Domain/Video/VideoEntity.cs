using StreamingProject.Domain.Stream;

namespace StreamingProject.Domain.Video;

public class VideoEntity
{

    private VideoEntity(Guid id, Guid streamId, Guid userId, string title, string fileUrl, string hlsUrl,DateTime createdAt)
    {
        Id = id;
        StreamId = streamId;
        UserId = userId;
        Title = title;
        FileUrl = fileUrl;
        HlsUrl = hlsUrl;
        CreatedAt = createdAt;
    }
    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;
    public string FileUrl { get; set; } = null!;

    public string HlsUrl { get; set; } = null!;


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StreamEntity Stream { get; set; } = null!;

    public static VideoEntity Create(string title, Guid streamId, Guid userId, string fileUrl, string HlsUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        }

        var video = new VideoEntity(
            Guid.NewGuid(),
            streamId,
            userId,
            title,
            fileUrl,
            HlsUrl,
            DateTime.UtcNow
            );
        
        return video;
    }
}