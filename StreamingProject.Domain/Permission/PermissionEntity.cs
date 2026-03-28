using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.Permission;

public class PermissionEntity
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    
    public ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();

    private PermissionEntity() { }

    private PermissionEntity(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static PermissionEntity Create(int id, string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("Permission name cannot be empty");
        
        return new PermissionEntity(id, name);
    }
}