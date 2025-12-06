using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Infrastructure.PasswordHasher.AuthorizeAttributes;

public class AuthorizeCreateAttribute : AuthorizeAttribute
{
    public AuthorizeCreateAttribute()
    {
        Policy = "Permission.Create";
    }
}