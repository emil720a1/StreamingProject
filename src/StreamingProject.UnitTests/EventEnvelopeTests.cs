using System.Text.Json;
using StreamingProject.Contracts.Events;

namespace StreamingProject.UnitTests;

[TestFixture]
public class EventEnvelopeTests
{
    [Test]
    public void Create_stream_started()
    {
        var streamStartedPayload = new StreamStartedPayload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var eventEnvelope = new EventEnvelope<StreamStartedPayload>(
            Guid.NewGuid(),
            "stream.started",
            1,
            DateTimeOffset.UtcNow,
            "media-service",
            Guid.NewGuid().ToString(),
            null,
            streamStartedPayload);

        Assert.That(eventEnvelope.EventType, Is.EqualTo("stream.started"));
        Assert.That(eventEnvelope.EventVersion, Is.EqualTo(1));
        Assert.That(eventEnvelope.Producer, Is.EqualTo("media-service"));
        Assert.That(eventEnvelope.Payload, Is.EqualTo(streamStartedPayload));
    }

    [Test]
    public void Create_stream_ended()
    {
        var streamEndedPayload = new StreamEndedPayload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            1800,
            "publisher_disconnected");

        var eventEnvelope = new EventEnvelope<StreamEndedPayload>(
            Guid.NewGuid(),
            "stream.ended",
            1,
            DateTimeOffset.UtcNow,
            "media-service",
            Guid.NewGuid().ToString(),
            null,
            streamEndedPayload);

        Assert.That(eventEnvelope.EventType, Is.EqualTo("stream.ended"));
        Assert.That(eventEnvelope.EventVersion, Is.EqualTo(1));
        Assert.That(eventEnvelope.Producer, Is.EqualTo("media-service"));
        Assert.That(eventEnvelope.Payload, Is.EqualTo(streamEndedPayload));
        Assert.That(
            eventEnvelope.Payload.DurationSeconds,
            Is.EqualTo(1800));
        Assert.That(eventEnvelope.Payload.Reason,
            Is.EqualTo("publisher_disconnected"));
    }

    [Test]
    public void Serialize_stream_started_to_json()
    {
        var payload = new StreamStartedPayload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var envelope = new EventEnvelope<StreamStartedPayload>(
            Guid.NewGuid(),
            "stream.started",
            1,
            DateTimeOffset.UtcNow,
            "media-service",
            Guid.NewGuid().ToString(),
            null,
            payload);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(envelope, options);

        Assert.That(json, Does.Contain("\"eventType\":\"stream.started\""));
        Assert.That(json, Does.Contain("\"producer\":\"media-service\""));
        Assert.That(json, Does.Contain("\"payload\""));
        Assert.That(json, Does.Contain("\"streamId\""));
        Assert.That(json, Does.Not.Contain("streamKey"));
    }
}
