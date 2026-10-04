using Microsoft.AspNetCore.Authorization;
using StreamingProject.Repository.Authentication;

namespace StreamingProject.IntegrationTests;

public sealed class TestPermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasReadPermission = context.User.HasClaim(
            "test_permission",
            "Read");

        if (hasReadPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
