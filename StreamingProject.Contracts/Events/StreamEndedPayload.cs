namespace StreamingProject.Contracts.Events;

public sealed record StreamEndedPayload(
    Guid StreamId,
    Guid MediaSessionId,
    DateTimeOffset EndedAt,
    int DurationSeconds,
    string Reason);