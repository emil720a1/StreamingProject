using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Application.Service.Permission.PermissionRepository;

public interface IPermissionRepository
{
    Task<PermissionEntity> AddPermissionAsync(PermissionEntity permission);
    Task<List<PermissionEntity>> GetPermissionsAsync(Guid userId);
    
    Task<bool> RemovePermissionAsync(Guid userId, int permissionId);
    
}