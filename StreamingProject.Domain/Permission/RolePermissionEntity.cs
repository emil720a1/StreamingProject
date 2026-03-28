using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.Permission;

public class RolePermissionEntity
{
    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }
    public virtual RoleEntity Role { get; private set; }
    public virtual PermissionEntity Permission { get; private set; }
    
    private RolePermissionEntity() { }

    public RolePermissionEntity(int roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}