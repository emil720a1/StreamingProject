namespace StreamingProject.Contracts.Events;

public sealed record EventEnvelope<TPayload>(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAt,
    string Producer,
    string CorrelationId,
    string? CausationId,
    TPayload Payload);
