using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Infrastructure.PasswordHasher.AuthorizeAttributes;

public class AuthorizeUpdateAttribute : AuthorizeAttribute
{
    public AuthorizeUpdateAttribute()
    {
        Policy = "Permission.Update";
        
    }
}