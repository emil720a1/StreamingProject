namespace StreamingProject.Domain;

public class ChatEntity
{
    public Guid Id { get; set; }
    
    public StreamEntity Stream { get; set; }
    
    
    public string Message { get; set; }
    
    public DateTime SentTime { get; set; }
    
    public Guid UserId { get; set; }
    
}