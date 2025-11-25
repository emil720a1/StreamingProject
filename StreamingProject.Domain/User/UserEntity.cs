namespace StreamingProject.Domain.User;

public class UserEntity
{

   private UserEntity(Guid id, string username, string password, string email, UserRole role, UserStatus status)
   {
      Id = id;
      Username = username;
      Password = password;
      Email = email;
      Role = role;
      Status = status;
   }
   public Guid Id { get; set; }
   
   public string Username { get; set; }
   
   public string Password { get; set; }
   
   public string FirstName { get; set; }
   
   public string LastName { get; set; }
   
   public IEnumerable<StreamEntity> Streams {get; set;}

   public string Email { get; set; }
   public UserRole Role { get; set; }
   
   public UserStatus Status { get; set; }


   public static UserEntity Create(string Username, string Password, string Email)
   {
      if (string.IsNullOrWhiteSpace(Username))
      {
         throw new ArgumentException("Username cannot be null or whitespace", nameof(Username));
      }
      
      var id = Guid.NewGuid();
      var role = UserRole.User;
      var status = UserStatus.Active;
      
      

      return new UserEntity(id, Username, Password, Email, role, status);
   }
}