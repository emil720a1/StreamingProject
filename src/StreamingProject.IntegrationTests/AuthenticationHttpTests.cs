using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.Authentication;

namespace StreamingProject.IntegrationTests;

[TestFixture]
public class AuthenticationHttpTests
{
    [Test]
    public async Task CorsPreflight_ShouldAllowCredentialsFromFrontend()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/Auth/login");

        request.Headers.Add("Origin", "http://localhost:4200");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.Headers.GetValues("Access-Control-Allow-Origin")
            .Should().Contain("http://localhost:4200");
        response.Headers.GetValues("Access-Control-Allow-Credentials")
            .Should().Contain("true");
    }

    [Test]
    public async Task CookieAuthenticationFlow_ShouldLoginAuthenticateAndLogout()
    {
        await using var factory =
            new CookieAuthenticationWebApplicationFactory("Development");
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
            });

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new LoginUserRequest("integration@example.com", "password"));

        loginResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var loginCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();

        AssertDevelopmentCookie(
            loginCookies,
            AuthenticationCookieOptionsFactory.AccessTokenCookieName);
        AssertDevelopmentCookie(
            loginCookies,
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName);

        var authenticatedResponse = await client.GetAsync("/api/Streams");
        authenticatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var logoutResponse = await client.PostAsync(
            "/api/Auth/logout",
            content: null);

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var expiredCookies = logoutResponse.Headers
            .GetValues("Set-Cookie")
            .Select(value => value.ToLowerInvariant())
            .ToList();

        expiredCookies.Should().Contain(cookie =>
            cookie.StartsWith(
                AuthenticationCookieOptionsFactory.AccessTokenCookieName));
        expiredCookies.Should().Contain(cookie =>
            cookie.StartsWith(
                AuthenticationCookieOptionsFactory.RefreshTokenCookieName));
        expiredCookies.Should().OnlyContain(cookie =>
            cookie.Contains("expires=thu, 01 jan 1970"));

        var responseAfterLogout = await client.GetAsync("/api/Streams");
        responseAfterLogout.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Login_ShouldSetSecureCookiesInProduction()
    {
        await using var factory =
            new CookieAuthenticationWebApplicationFactory("Production");
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                HandleCookies = false,
            });

        var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new LoginUserRequest("integration@example.com", "password"));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var cookies = response.Headers.GetValues("Set-Cookie").ToList();

        AssertProductionCookie(
            cookies,
            AuthenticationCookieOptionsFactory.AccessTokenCookieName);
        AssertProductionCookie(
            cookies,
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName);
    }

    [Test]
    public async Task BearerTokenWithoutCookie_ShouldNotAuthenticate()
    {
        await using var factory =
            new CookieAuthenticationWebApplicationFactory("Development");
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                HandleCookies = false,
            });

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new LoginUserRequest("integration@example.com", "password"));
        var accessCookie = loginResponse.Headers
            .GetValues("Set-Cookie")
            .Single(value => value.StartsWith(
                $"{AuthenticationCookieOptionsFactory.AccessTokenCookieName}=",
                StringComparison.Ordinal));
        var accessToken = accessCookie
            .Split(';', 2)[0]
            .Split('=', 2)[1];

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("/api/Streams");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static void AssertDevelopmentCookie(
        IEnumerable<string> cookies,
        string cookieName)
    {
        var cookie = cookies.Single(value =>
            value.StartsWith($"{cookieName}=", StringComparison.Ordinal));
        var normalizedCookie = cookie.ToLowerInvariant();

        normalizedCookie.Should().Contain("httponly");
        normalizedCookie.Should().Contain("samesite=lax");
        normalizedCookie.Should().NotContain("secure");
        AssertCookiePath(normalizedCookie, cookieName);
    }

    private static void AssertProductionCookie(
        IEnumerable<string> cookies,
        string cookieName)
    {
        var cookie = cookies.Single(value =>
            value.StartsWith($"{cookieName}=", StringComparison.Ordinal));
        var normalizedCookie = cookie.ToLowerInvariant();

        normalizedCookie.Should().Contain("httponly");
        normalizedCookie.Should().Contain("samesite=none");
        normalizedCookie.Should().Contain("secure");
        AssertCookiePath(normalizedCookie, cookieName);
    }

    private static void AssertCookiePath(
        string normalizedCookie,
        string cookieName)
    {
        var expectedPath = cookieName ==
            AuthenticationCookieOptionsFactory.RefreshTokenCookieName
                ? "/api/auth/refresh-token"
                : "/";

        normalizedCookie.Should().Contain($"path={expectedPath}");
    }
}
