using StreamingProject.Domain.User;

namespace StreamingProject.Application;

public interface IUserRepository
{

    Task<UserEntity> AddUserAsync(UserEntity user);
    
    Task<UserEntity> UpdateUserAsync(UserEntity user);
    
    Task<bool> DeleteUserAsync(Guid id);
    
    Task<UserEntity?> GetUserById(Guid id);

    Task<UserEntity?> GetStreamsByUserId(Guid userId);
    
    Task<bool> UserExists(string username);
    
    Task<UserEntity> GetByEmail(string email);
}