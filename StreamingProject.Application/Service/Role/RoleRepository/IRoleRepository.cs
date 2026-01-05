using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.Role.RoleRepository;

public interface IRoleRepository
{
    Task<RoleEntity?> AddRoleAsync(RoleEntity role);
    
    Task<RoleEntity?> GetRoleByNameAsync(string name);
    
    Task<RoleEntity?> GetRoleByIdAsync(int id);
    
    Task<RoleEntity> UpdateRoleAsync(RoleEntity role);
    
    Task<bool> DeleteRoleAsync(int id);
    
    
}