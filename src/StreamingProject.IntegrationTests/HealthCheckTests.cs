using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace StreamingProject.IntegrationTests;

[TestFixture]
public class HealthCheckTests
{
    [Test]
    public async Task Health_ShouldReturnOk_WhenAllChecksAreHealthy()
    {
        await using var baseFactory = new TestWebApplicationFactory();
        await using var factory = baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IConfigureOptions<HealthCheckServiceOptions>>();
                services.AddHealthChecks()
                    .AddCheck(
                        "test",
                        () => HealthCheckResult.Healthy());
            });
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Health_ShouldReturnServiceUnavailable_WhenPostgresIsUnavailable()
    {
        await using var baseFactory = new TestWebApplicationFactory();
        await using var factory = baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:StreamingDbContext"] =
                            "Host=127.0.0.1;Port=1;Database=unavailable;" +
                            "Username=test;Password=test;Timeout=1",
                    });
            });
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
