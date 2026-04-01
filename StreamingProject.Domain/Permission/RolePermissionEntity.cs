using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.Permission;

public class RolePermissionEntity
{
    public int RoleId { get; init; }
    public RoleEntity Role { get; private set; }
    public int PermissionId { get; init; }
    public PermissionEntity Permission { get; private set; }
    
    public RolePermissionEntity() { }

    public RolePermissionEntity(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}