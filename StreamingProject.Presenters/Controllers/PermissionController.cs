using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Contracts.Permissions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController(IPermissionService permissionService) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Create")]
    [HttpPost("add")]
    public async Task<IActionResult> AddPermission(
        [FromBody] AddPermissionDto request,
        CancellationToken cancellationToken)
    {
        var result = await permissionService.AddPermissionAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("all")]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] GetPermissionsDto request,
        CancellationToken cancellationToken)
    {
        var result = await permissionService.GetPermissionsAsync(request, cancellationToken);
        return HandleResult(result);
    }
}