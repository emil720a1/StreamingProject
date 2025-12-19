namespace StreamingProject.Domain.Stream.UserStream;

public class UserStream
{
    public Guid UserId { get; set; }
    
    public Guid StreamId { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    
}