using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using StreamingProject.Presenters.Authentication;

namespace StreamingProject.UnitTests;

[TestFixture]
public class AuthenticationCookieOptionsFactoryTests
{
    [TestCase("Development", false, SameSiteMode.Lax)]
    [TestCase("Production", true, SameSiteMode.None)]
    public void Create_ShouldUseEnvironmentSpecificPolicy(
        string environmentName,
        bool expectedSecure,
        SameSiteMode expectedSameSite)
    {
        var environment = new Mock<IHostEnvironment>();
        environment
            .SetupGet(item => item.EnvironmentName)
            .Returns(environmentName);

        var factory = new AuthenticationCookieOptionsFactory(environment.Object);

        var options = factory.CreateAccessTokenOptions();

        options.Secure.Should().Be(expectedSecure);
        options.SameSite.Should().Be(expectedSameSite);
        options.HttpOnly.Should().BeTrue();
        options.Path.Should().Be("/");
        options.Expires.Should().NotBeNull();
    }

    [Test]
    public void RefreshTokenCookie_ShouldLiveLongerThanAccessTokenCookie()
    {
        var environment = new Mock<IHostEnvironment>();
        environment
            .SetupGet(item => item.EnvironmentName)
            .Returns("Development");

        var factory = new AuthenticationCookieOptionsFactory(environment.Object);

        var accessTokenOptions = factory.CreateAccessTokenOptions();
        var refreshTokenOptions = factory.CreateRefreshTokenOptions();

        refreshTokenOptions.Expires.Should().BeAfter(
            accessTokenOptions.Expires!.Value);
        refreshTokenOptions.Path.Should().Be("/api/Auth/refresh-token");
    }
}
