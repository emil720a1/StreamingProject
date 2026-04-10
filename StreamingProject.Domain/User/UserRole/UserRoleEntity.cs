using Microsoft.AspNetCore.Identity;

namespace StreamingProject.Domain.User.UserRole;

public class UserRoleEntity : IdentityUserRole<Guid>
{
    public virtual UserEntity User { get; set; } = null!;
    public virtual RoleEntity Role { get; set; } = null!;
}