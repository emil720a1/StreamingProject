using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Repository.AuthorizeAttributes;

public class AuthorizeUpdateAttribute : AuthorizeAttribute
{
    public AuthorizeUpdateAttribute()
    {
        Policy = "Permission.Update";
        
    }
}