using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Domain.User;

namespace StreamingProject.Application.Interfaces.Auth;

public interface IJwtProvider
{
    Task<TokenResponse> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}