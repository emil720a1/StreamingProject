using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace StreamingProject.Presenters.Authentication;

public sealed class AuthenticationCookieOptionsFactory(
    IHostEnvironment environment)
{
    public const string AccessTokenCookieName = "tasty-cookies";
    public const string RefreshTokenCookieName = "tasty-refresh-token";

    public CookieOptions CreateAccessTokenOptions() =>
        Create(TimeSpan.FromHours(1), "/");

    public CookieOptions CreateRefreshTokenOptions() =>
        Create(TimeSpan.FromDays(7), "/api/Auth/refresh-token");

    private CookieOptions Create(TimeSpan lifetime, string path)
    {
        var isDevelopment = environment.IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment
                ? SameSiteMode.Lax
                : SameSiteMode.None,
            Path = path,
            Expires = DateTimeOffset.UtcNow.Add(lifetime),
        };
    }
}
