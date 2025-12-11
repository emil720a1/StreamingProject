using Microsoft.AspNetCore.Authorization;

namespace StreamingProject.Repository.AuthorizeAttributes;

public class AuthorizeDeleteAttribute : AuthorizeAttribute
{
    public AuthorizeDeleteAttribute()
    {
        Policy = "Permission.Delete";
        
    }
}