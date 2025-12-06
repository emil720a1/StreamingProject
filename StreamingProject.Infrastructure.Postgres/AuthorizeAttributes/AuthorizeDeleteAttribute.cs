using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Infrastructure.PasswordHasher.AuthorizeAttributes;

public class AuthorizeDeleteAttribute : AuthorizeAttribute
{
    public AuthorizeDeleteAttribute()
    {
        Policy = "Permission.Delete";
        
    }
}