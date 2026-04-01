using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;
using StreamingProject.Domain.Subscription;
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

   public UserEntity()
   {
      UserRoles = new List<UserRoleEntity>();
      Streams = new List<StreamEntity>();
   }
   
   public Guid Id { get; private set; }
   
   public string Username { get; private set; }
   
   public string PasswordHash { get; private set; }
   
   public string FirstName { get; set; } 
   
   public string LastName { get; set; }

   private readonly List<UserStream> _joinedStreams = new();
   public IReadOnlyList<UserStream> JoinedStreams => _joinedStreams;

   private readonly List<SubscriptionEntity> _followers = new();
   public IReadOnlyList<SubscriptionEntity> Followers => _followers;
   
   private readonly List<SubscriptionEntity> _followings = new();
   public IReadOnlyList<SubscriptionEntity> Followings => _followings;
   
   public ICollection<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();
   public ICollection<StreamEntity> Streams {get; private set;} = new List<StreamEntity>();

   public string Email { get; set; }
   
   public UserStatus Status { get; set; }


   public static UserEntity Create(
      string username,
      string passwordHash,
      string email, 
      RoleEntity role,
      string firstName,
      string lastName)
   {
      if (string.IsNullOrWhiteSpace(username))
         throw new ArgumentException("Username cannot be null or whitespace", nameof(username));

      var user = new UserEntity
      {
         Id = Guid.NewGuid(),
         Username = username,
         PasswordHash = passwordHash,
         Email = email,
         FirstName = firstName,
         LastName = lastName
      };
      
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