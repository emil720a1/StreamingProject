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

        var options = factory.Create();

        options.Secure.Should().Be(expectedSecure);
        options.SameSite.Should().Be(expectedSameSite);
        options.HttpOnly.Should().BeTrue();
        options.Path.Should().Be("/");
        options.Expires.Should().NotBeNull();
    }
}
