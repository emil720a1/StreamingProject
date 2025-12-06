using StreamingProject.Domain.Stream;

namespace StreamingProject.Domain.Video;

public class VideoEntity
{
    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }
    
    public string FileUrl { get; set; }
    
    public string HlsUrl { get; set; }
    
    public StreamEntity Stream { get; set; }
    
}