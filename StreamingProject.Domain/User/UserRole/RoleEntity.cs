using Microsoft.AspNetCore.Identity;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Domain.User.UserRole;

public class RoleEntity : IdentityRole<Guid>
{
   //EF-Core
   public RoleEntity() : base() { }
   
   private RoleEntity(Guid id, string name) : base(name)
   {
      Id = id;
      Name = name;
   }
   
   public ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
   
   public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();

   public static RoleEntity Create(Guid id, string name)
   {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or empty");
      
      return new RoleEntity
      {
         Id = id,
         Name = name,
         Permissions = new List<PermissionEntity>()
      };
   }
}
