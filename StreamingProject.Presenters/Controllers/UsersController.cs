using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, ICurrentUser currentUser) : ApiControllerBase
{


    [Authorize(Policy = "Permission.Read")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var request = new GetUserDto(currentUser.Id);
        var result = await userService.GetUserByIdAsync(request, cancellationToken);
        
        return HandleResult(result);
    }
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("streams")]
    public async Task<IActionResult> GetStreamsByUserId(CancellationToken cancellationToken)
    {
        var request = new GetUserDto(currentUser.Id);
        var result = await userService.GetStreamsByUserIdAsync(request, cancellationToken);
        
        return HandleResult(result);
    }
}