using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Role.RoleService;
using StreamingProject.Contracts.Roles;
using StreamingProject.Contracts.VideoDto.Crud;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters;



[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }


    [Authorize(Policy = "Permission.Create")]
    [HttpPost("add")]

    public async Task<IActionResult> AddRole(
        [FromBody] AddRoleDto request,
        CancellationToken cancellationToken)
    {

        var result = await _roleService.AddRoleAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);

    }
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getByName")]

    public async Task<IActionResult> GetRoleByName(
        [FromQuery] GetRoleByNameDto request,
        CancellationToken cancellationToken)
    {

        var result = await _roleService.GetRoleByName(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getById")]

    public async Task<IActionResult> GetRoleById(
        [FromQuery] GetRoleByIdDto request,
        CancellationToken cancellationToken)
    {
        var result = await _roleService.GetRoleById(request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }


    [Authorize(Policy = "Permission.Update")]
    [HttpPost("update")]

    public async Task<IActionResult> UpdateRole(
        [FromBody] UpdateRoleDto request,
        CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdateRoleAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    
    
    [AllowAnonymous]
    // [Authorize(Policy = "Permission.Delete")]
    [HttpPost("delete")]

    public async Task<IActionResult> DeleteRole(
        [FromBody] DeleteRoleDto request,
        CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteRoleAsync(request, cancellationToken);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
        
    }
    
    
}