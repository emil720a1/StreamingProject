namespace StreamingProject.Domain.Stream;

public class StreamLikeEntity
{
    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime? LikeTime { get; set; }
    
    
}