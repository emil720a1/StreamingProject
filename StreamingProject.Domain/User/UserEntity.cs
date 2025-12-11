using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Domain.User;

public class UserEntity
{

   private UserEntity(Guid id, string username, string password, string email, UserStatus status, string firstName, string lastName)
   {
      Id = id;
      Username = username;
      Password = password;
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
   
   public Guid Id { get; set; }
   
   public string Username { get; set; }
   
   public string Password { get; set; }
   
   public string FirstName { get; set; } 
   
   public string LastName { get; set; } 
   
   public IEnumerable<StreamEntity> Streams {get; set;}

   public string Email { get; set; }
   public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
   
   public UserStatus Status { get; set; }


   public static UserEntity Create(string Username, string Password, string Email, RoleEntity Role)
   {
      if (string.IsNullOrWhiteSpace(Username))
      {
         throw new ArgumentException("Username cannot be null or whitespace", nameof(Username));
      }
      
      var id = Guid.NewGuid();
      var status = UserStatus.Active;

      var user = new UserEntity(id, 
         Username, 
         Password, 
         Email, 
         status, 
         string.Empty,
         string.Empty
         );


      var userRole = new UserRoleEntity
      {
         UserId = user.Id,
         User = user,
         RoleId = Role.Id,
         Role = Role
      };
      
      user.UserRoles.Add(userRole);
      
      return user;
   }
}