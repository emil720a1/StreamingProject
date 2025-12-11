using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Repository.AuthorizeAttributes;

public class AuthorizeReadAttribute : AuthorizeAttribute
{
    public AuthorizeReadAttribute()
    {
        Policy = "Permission.Read";
    }
}