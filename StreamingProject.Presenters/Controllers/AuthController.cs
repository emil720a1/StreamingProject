using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.Authentication;
using StreamingProject.Presenters.ResponseExtensions;

namespace StreamingProject.Presenters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IUserService userService,
    AuthenticationCookieOptionsFactory cookieOptionsFactory)
    : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var addUserDto = new AddUserDto(
            request.Username,
            request.Password,
            request.Email,
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

        AppendAuthenticationCookies(result.Value);

        return NoContent();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var result = await userService.RefreshTokenAsync(refreshToken, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        AppendAuthenticationCookies(result.Value);

        return NoContent();
    }

    [Authorize]
    [HttpGet("session")]
    public IActionResult Session()
    {
        return NoContent();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(
            AuthenticationCookieOptionsFactory.AccessTokenCookieName,
            cookieOptionsFactory.CreateAccessTokenOptions());

        Response.Cookies.Delete(
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName,
            cookieOptionsFactory.CreateRefreshTokenOptions());

        return NoContent();
    }

    private void AppendAuthenticationCookies(TokenResponse tokenResponse)
    {
        Response.Cookies.Append(
            AuthenticationCookieOptionsFactory.AccessTokenCookieName,
            tokenResponse.AccessToken,
            cookieOptionsFactory.CreateAccessTokenOptions());

        Response.Cookies.Append(
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName,
            tokenResponse.RefreshToken,
            cookieOptionsFactory.CreateRefreshTokenOptions());
    }
}
