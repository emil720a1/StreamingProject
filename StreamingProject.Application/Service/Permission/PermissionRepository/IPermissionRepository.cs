using StreamingProject.Domain.Permission;

namespace StreamingProject.Application.Service.Permission.PermissionRepository;

public interface IPermissionRepository
{
    Task<PermissionEntity> AddPermissionAsync(int userId, PermissionEntity permission);
    Task<List<PermissionEntity>> GetPermissionsAsync(int userId);
    
    Task<PermissionEntity?> GetPermissionAsync(int userId);
    Task<PermissionEntity> UpdatePermissionAsync(int userId, PermissionEntity permission);
    Task<bool> RemovePermissionAsync(int userId, PermissionEntity permission);
    
}