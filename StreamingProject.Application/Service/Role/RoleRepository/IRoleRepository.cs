using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.Role.RoleRepository;

public interface IRoleRepository
{
    Task<RoleEntity> GetRoleByNameAsync(string username);
    
    Task<RoleEntity?> GetRoleByIdAsync(int id);
    
    
}