    namespace StreamingProject.Domain.User.UserRole;

    public class UserRoleEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        
        
        public int RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }
    }