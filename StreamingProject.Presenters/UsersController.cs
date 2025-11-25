using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application;
using StreamingProject.Application.User;
using StreamingProject.Application.User.AuthDto;
using StreamingProject.Presenters.ResponseExtensions;

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

    [HttpPost("register")]

    public async Task<IActionResult> Register(
      [FromBody]  RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.Register(request.Username, request.Email, request.Password);
        
        return result.IsFailure 
            ? result.Error.ToResponse() 
            : Ok(result.Value);
    }
    
    [HttpPost("login")]

    public async Task<IActionResult> Login(
      [FromBody]  LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var token = await _userService.Login(request.Email, request.Password);

        return token.IsFailure 
            ? token.Error.ToResponse() 
            : Ok(token.Value);
    }

    
}