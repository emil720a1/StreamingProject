using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Repository.AuthorizeAttributes;

public class AuthorizeCreateAttribute : AuthorizeAttribute
{
    public AuthorizeCreateAttribute()
    {
        Policy = "Permission.Create";
    }
}