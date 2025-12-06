using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Contracts.Permissions;
using StreamingProject.Infrastructure.PasswordHasher.Authentication;

namespace StreamingProject.Repository.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == CustomClaims.UserId);
        
        if (userId is null || !Guid.TryParse(userId.Value, out var id))
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();

        var permissionService = scope.ServiceProvider
            .GetRequiredService<IPermissionService>();

        var request = new GetPermissionsDto(id);
        
        var permissions = await permissionService.GetPermissionsAsync(userId);

        if (permissions.Intersect(requirement.Permissions).Any())
        {
            context.Succeed(requirement);
        }
    }
}