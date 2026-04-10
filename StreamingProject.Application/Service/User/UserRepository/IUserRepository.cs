using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User;

namespace StreamingProject.Application.Service.User.UserRepository;

public interface IUserRepository
{

    Task<UserEntity> AddUserAsync(UserEntity user);
    
    Task<UserEntity> UpdateUserAsync(UserEntity user);
    
    Task<bool> DeleteUserAsync(Guid id);
    
    Task<UserEntity?> GetUserById(Guid id);

    Task<IEnumerable<StreamEntity>> GetStreamsByUserId(Guid userId);
    
    Task<bool> UserExists(string username, string email);
    
    Task<UserEntity> GetByEmail(string email);
    
}