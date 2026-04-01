using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Service.User.UserService;

public interface IUserService
{
    Task<Result<UserDetailsDto, Failure>> RegisterAsync(AddUserDto request, CancellationToken cancellationToken);
    Task<Result<string, Failure>> LoginAsync(string email, string password);
    
    Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserIdAsync(GetUserDto request, CancellationToken cancellationToken);
    Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(GetUserDto request, CancellationToken cancellationToken);
}