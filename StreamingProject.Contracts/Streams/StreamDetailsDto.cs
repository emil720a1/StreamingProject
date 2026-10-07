namespace StreamingProject.Contracts.Streams;

public record StreamDetailsDto(
    Guid Id, 
    Guid UserId, 
    string? StreamerUsername,
    string Title,
    string Description,
    DateTime? StartTime,
    DateTime? EndTime);
