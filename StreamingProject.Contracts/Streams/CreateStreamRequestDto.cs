namespace StreamingProject.Contracts.Streams;

public record CreateStreamRequestDto(
    string Title,
    string Description,
    string Category,
    string? ThumbnailUrl);