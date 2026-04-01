using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;


[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("register")]

    public async Task<IActionResult> Register(
      [FromBody]  RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var addUserDto = new AddUserDto(
            request.Username, 
            request.Email, 
            request.Password, 
            null, 
            null);
        
        var result = await _userService.RegisterAsync(addUserDto, cancellationToken);
        
        return result.IsFailure 
            ? result.Error.ToResponse() 
            : Ok(result.Value);
    }
    
    [HttpPost("login")]

    public async Task<IActionResult> Login(
      [FromBody]  LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        
        HttpContext.Response.Cookies.Append("tasty-cookies", result.Value);
        return Ok(result.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var request = new GetUserDto(userId);
        
        var user = await _userService.GetUserByIdAsync(request, cancellationToken);
        
        return user.IsFailure ? user.Error.ToResponse() : Ok(user.Value);
    }
    
    
    [Authorize(Policy = "Permission.Read")]
    [HttpGet("streams")]
    public async Task<IActionResult> GetStreamsByUserId(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        var request = new GetUserDto(userId);
        
        var streams = await _userService.GetStreamsByUserIdAsync(request, cancellationToken);
        
        return streams.IsFailure ? streams.Error.ToResponse() : Ok(streams.Value);
    }
    
    
}