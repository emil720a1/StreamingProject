namespace StreamingProject.Contracts.Streams;

public sealed record PaginatedResponseDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int CurrentPage,
    int PageSize,
    int TotalPages);
