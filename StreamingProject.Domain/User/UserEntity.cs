using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.User;

public class UserEntity
{

   private UserEntity(
      Guid id, 
      string username, 
      string passwordHash, 
      string email, 
      UserStatus status, 
      string firstName, 
      string lastName)
   {
      Id = id;
      Username = username;
      PasswordHash = passwordHash;
      Email = email;
      Status = status;
      FirstName = firstName;
      LastName = lastName;
      
      
      UserRoles = new List<UserRoleEntity>();
      Streams = new List<StreamEntity>();
   }

   private UserEntity()
   {
      UserRoles = new List<UserRoleEntity>();
      Streams = new List<StreamEntity>();
   }
   
   public Guid Id { get; private set; }
   
   public string Username { get; private set; }
   
   public string PasswordHash { get; private set; }
   
   public string FirstName { get; set; } 
   
   public string LastName { get; set; } 
   
   public ICollection<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();
   public ICollection<StreamEntity> Streams {get; private set;} = new List<StreamEntity>();

   public string Email { get; set; }
   
   public UserStatus Status { get; set; }


   public static UserEntity Create(string username, string passwordHash, string email, RoleEntity role)
   {
      if (string.IsNullOrWhiteSpace(username))
         throw new ArgumentException("Username cannot be null or whitespace", nameof(username));

      var user = UserEntity.Create(username, passwordHash, email, role);
      
      user.UserRoles.Add(new UserRoleEntity
      {
         UserId = user.Id,
         User = user,
         RoleId = role.Id,
         Role = role
      });
      
      return user;
   }
}