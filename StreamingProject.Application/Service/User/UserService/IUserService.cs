using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.User;

public interface IUserService
{
    Task<Result<UserDetailsDto, Failure>> AddUserAsync(AddUserDto userDto, CancellationToken cancellationToken);
    
    Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(GetUserDto userDto, CancellationToken cancellationToken);
    
    Task<Result<UserDetailsDto, Failure>> UpdateUserAsync(UpdateUserDto userDto, CancellationToken cancellationToken);

    Task<Result<bool, Failure>> DeleteUserAsync(DeleteUserDto userDto, CancellationToken cancellationToken);
    
    Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserId(GetStreamsByUserId request, CancellationToken cancellationToken);


    Task <Result<UserDetailsDto, Failure>> Register(string userName,string email, string PasswordHash);
    Task<Result<string, Failure>> Login(string email, string password);
}