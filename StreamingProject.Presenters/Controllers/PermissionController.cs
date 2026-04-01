using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Contracts.Permissions;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;


[ApiController]
[Route ("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [Authorize(Policy = "Permission.Create")]
    [HttpPost("add")]
    public async Task<IActionResult> AddPermission(
        [FromBody] AddPermissionDto request,
        CancellationToken cancellationToken)
    {
        var result = await _permissionService.AddPermissionAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("all")]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] GetPermissionsDto request,
        CancellationToken cancellationToken)
    {
        var result = await _permissionService.GetPermissionsAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}