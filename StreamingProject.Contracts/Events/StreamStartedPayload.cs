namespace StreamingProject.Contracts.Events;

public sealed record StreamStartedPayload(
    Guid StreamId,
    Guid MediaSessionId,
    DateTimeOffset StartedAt);