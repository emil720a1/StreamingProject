namespace StreamingProject.Contracts.Streams;

public sealed class GetStreamsRequestDto
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 12;

    public string? Category { get; init; }

    public string? Search { get; init; }

    public string SortBy { get; init; } = "startTime";

    public string SortDirection { get; init; } = "desc";
}
