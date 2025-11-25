using StreamingProject.Domain.User;

namespace StreamingProject.Infrastructure.PasswordHasher;

public interface IJwtProvider
{
    string GenerateToken(UserEntity user);
}