using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Contracts.Permissions;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters;


[ApiController]
[Route ("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<IActionResult> AddPermission(
        [FromBody] AddPermissionDto request,
        CancellationToken cancellationToken)
    {
        var result = await _permissionService.AddPermissionAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    public async Task<IActionResult> GetPermissions(
        [FromQuery] GetPermissionsDto request,
        CancellationToken cancellationToken)
    {
        var result = await _permissionService.GetPermissionsAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    public async Task<IActionResult> UpdatePermission(
        [FromBody] UpdatePermissionDto request,
        CancellationToken cancellationToken)
    {
        var result = await _permissionService.UpdatePermissionAsync(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
        
        
    }
}