namespace StreamingProject.Application.Service.Stream.StreamRepository;

public sealed record StreamPage(
    IReadOnlyList<StreamListItemReadModel> Items,
    int TotalCount);

public sealed record StreamListItemReadModel(
    Guid Id,
    Guid UserId,
    string? StreamerUsername,
    string Title,
    string Description,
    DateTime? StartTime,
    string Category,
    string? ThumbnailUrl,
    int ViewerCount);
