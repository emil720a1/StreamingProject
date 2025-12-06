using StreamingProject.Domain.Permission;

namespace StreamingProject.Domain.User.UserRole;

public class RoleEntity
{
   public int Id { get; set; } 
   
   public string Name { get; set; } = string.Empty;

   public ICollection<PermissionEntity> Permissions { get; set; } = [];
   
   public ICollection<UserEntity> Users { get; set; } = [];
}