using System.Reflection.Metadata.Ecma335;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Domain.User.UserRole;

public class RoleEntity
{
   //EF-Core
  public  RoleEntity(){ }
   
   private RoleEntity(int id, string name, ICollection<PermissionEntity> permissions)
   {
      Name = name;
      Id = id;
      Permissions = permissions;
   }
   
   public int Id { get; set; } 
   
   public string Name { get; set; } = string.Empty;

   public ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
   
   public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();



   public static RoleEntity Create(int id, string name)
   {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or empty");

      if (id <= 0) throw new ArgumentException("Id must be greater than 0");
      
      return new RoleEntity
      {
         Id = id,
         Name = name,
         Permissions = new List<PermissionEntity>()
      };
   }
}
