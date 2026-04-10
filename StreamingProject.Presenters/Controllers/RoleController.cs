using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.Role.RoleService;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService) : ApiControllerBase
{
    [Authorize(Policy = "Permission.Create")]
    [HttpPost("add")]
    public async Task<IActionResult> AddRole(
        [FromBody] AddRoleDto request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.AddRoleAsync(request, cancellationToken);
        return HandleResult(result);
    }
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getByName")]
    public async Task<IActionResult> GetRoleByName(
        [FromQuery] GetRoleByNameDto request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetRoleByNameAsync(request, cancellationToken);
        return HandleResult(result);
    }
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("getById")]
    public async Task<IActionResult> GetRoleById(
        [FromQuery] GetRoleByIdDto request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetRoleByIdAsync(request, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Policy = "Permission.Update")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateRole(
        [FromBody] UpdateRoleDto request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.UpdateRoleAsync(request, cancellationToken);
        return HandleResult(result);
    }
    
    [Authorize(Policy = "Permission.Delete")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteRole(
        [FromBody] DeleteRoleDto request,
        CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRoleAsync(request, cancellationToken);
        return HandleResult(result);
    }
}