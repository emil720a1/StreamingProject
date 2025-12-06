using StreamingProject.Domain.User;

namespace StreamingProject.Application.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(UserEntity user);
}