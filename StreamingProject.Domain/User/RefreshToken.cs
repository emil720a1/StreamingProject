namespace StreamingProject.Domain.User;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }

    public bool IsActive => DateTime.UtcNow <= ExpiryDate && !IsRevoked;
}
