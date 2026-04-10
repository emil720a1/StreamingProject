namespace StreamingProject.Contracts.Streams;

public record StreamDetailsDto(
    Guid Id, 
    Guid UserId, 
    string? StreamerUsername,
    string Title,
    string Description,
    string StreamKey,
    DateTime? StartTime,
    DateTime? EndTime);