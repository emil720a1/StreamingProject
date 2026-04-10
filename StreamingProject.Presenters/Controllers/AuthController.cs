using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var addUserDto = new AddUserDto(
            request.Username, 
            request.Email, 
            request.Password, 
            null, 
            null);
        
        var result = await userService.RegisterAsync(addUserDto, cancellationToken);
        
        return HandleResult(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        
        HttpContext.Response.Cookies.Append("tasty-cookies", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        return Ok(new 
        { 
            Token = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken 
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        
        HttpContext.Response.Cookies.Append("tasty-cookies", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        return Ok(new 
        { 
            Token = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken 
        });
    }
}
