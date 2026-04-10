using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;

namespace StreamingProject.Application.Service.User.UserService;

public interface IUserService
{
    Task<Result<UserDetailsDto, Failure>> RegisterAsync(AddUserDto request, CancellationToken cancellationToken);
    Task<Result<TokenResponse, Failure>> LoginAsync(string email, string password);
    
    Task<Result<TokenResponse, Failure>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserIdAsync(GetUserDto request, CancellationToken cancellationToken);
    Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(GetUserDto request, CancellationToken cancellationToken);
}