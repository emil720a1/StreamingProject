using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Infrastructure.PasswordHasher.AuthorizeAttributes;

public class AuthorizeReadAttribute : AuthorizeAttribute
{
    public AuthorizeReadAttribute()
    {
        Policy = "Permission.Read";
    }
}