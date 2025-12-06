using Microsoft.AspNetCore.Authorization;
using StreamingProject.Domain.Enums;

namespace StreamingProject.Infrastructure.PasswordHasher.Authentication;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(PermissionEnum[] permissions)
    {
        Permissions = permissions;
    }
    
    public PermissionEnum[] Permissions { get; set; } = [];
}