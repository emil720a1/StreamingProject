using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace StreamingProject.Presenters.Authentication;

public sealed class AuthenticationCookieOptionsFactory(
    IHostEnvironment environment)
{
    public const string CookieName = "tasty-cookies";

    public CookieOptions Create()
    {
        var isDevelopment = environment.IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment
                ? SameSiteMode.Lax
                : SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        };
    }
}