using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.Permission;

public class PermissionEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public ICollection<RoleEntity> Roles { get; set; } = [];
}