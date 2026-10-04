using System.Net;
using FluentAssertions;

namespace StreamingProject.IntegrationTests;

[TestFixture]
public class StreamsControllerTests
{
    [Test]
    public async Task GetAvailableStreams_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        await using var factory = new TestWebApplicationFactory();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Streams");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetAvailableStreams_ShouldReturnForbidden_WhenUserLacksReadPermission()
    {
        await using var factory = new TestWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-Auth", "Create");

        var response = await client.GetAsync("/api/Streams");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task GetAvailableStreams_ShouldReturnOk_WhenUserHasReadPermission()
    {
        await using var factory = new TestWebApplicationFactory();

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Auth", "Read");

        var response = await client.GetAsync("/api/Streams");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("Test stream");
        body.Should().Contain("test-streamer");
        body.Should().NotContain("StreamKey");
    }
}
