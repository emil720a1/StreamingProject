using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.ResponseExtensions;
using StreamingProject.Repository.AuthorizeAttributes;

namespace StreamingProject.Presenters;


[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("/register")]

    public async Task<IActionResult> Register(
      [FromBody]  RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.Register(request.Username, request.Email, request.Password);
        
        return result.IsFailure 
            ? result.Error.ToResponse() 
            : Ok(result.Value);
    }
    
    [HttpPost("/login")]

    public async Task<IActionResult> Login(
      [FromBody]  LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var token = await _userService.Login(request.Email, request.Password);
        
        HttpContext.Response.Cookies.Append("tasty-cookies", token.Value);
        
        return token.IsFailure 
            ? token.Error.ToResponse() 
            : Ok(token.Value);
    }

    [Authorize(Policy = "Permission.Read")]
    [HttpGet("/get")]
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
    
    
    [AuthorizeRead]
    [HttpGet("/streams")]
    public async Task<IActionResult> GetStreamsByUserId(
        [FromRoute] GetUserDto request,
        CancellationToken cancellationToken)
    {
        var streams = await _userService.GetStreamsByUserId(request, cancellationToken);
        
        return streams.IsFailure ? streams.Error.ToResponse() : Ok(streams.Value);
    }
    
    
}