using System.Net;
using FluentAssertions;

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
    public async Task Logout_ShouldExpireAuthenticationCookie()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/Auth/logout", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var setCookie = response.Headers.GetValues("Set-Cookie").Single();
        setCookie.Should().Contain("tasty-cookies=");
        setCookie.Should().Contain("expires=Thu, 01 Jan 1970", Exactly.Once());
        setCookie.Should().Contain("path=/");
    }
}
