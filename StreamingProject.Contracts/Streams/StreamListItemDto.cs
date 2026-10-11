namespace StreamingProject.Contracts.Streams;

public record StreamListItemDto(
    Guid Id,
    Guid UserId,
    string? StreamerUsername,
    string Title,
    string Description,
    DateTime? StartTime,
    string Category,
    string? ThumbnailUrl,
    int ViewerCount);
