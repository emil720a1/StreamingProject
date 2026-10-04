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
}