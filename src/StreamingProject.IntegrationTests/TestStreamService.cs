using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.IntegrationTests;

public sealed class TestStreamService : IStreamService
{
    public Task<Result<PaginatedResponseDto<StreamListItemDto>, Failure>> GetAvailableStreamsAsync(
        GetStreamsRequestDto request,
        CancellationToken cancellationToken)
    {
        var streams = new List<StreamListItemDto>
        {
            new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "test-streamer",
                "Test stream",
                "Integration test stream",
                DateTime.UtcNow,
                "Gaming",
                "test-thumbnail.jpg",
                42)
        };

        var response = new PaginatedResponseDto<StreamListItemDto>(
            streams,
            1,
            request.Page,
            request.PageSize,
            1);

        return Task.FromResult(
            Result.Success<PaginatedResponseDto<StreamListItemDto>, Failure>(response));
    }

    public Task<Result<CreateStreamResponseDto, Failure>> CreateStreamAsync(
        CreateStreamDto streamDto,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(
        JoinStreamDto streamDto,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<StreamStatusDto, Failure>> GetStreamStatusAsync(
        Guid streamId,
        Guid userId,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<StreamDetailsDto, Failure>> GetStreamByIdAsync(
        GetStreamByIdDto streamDto,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<bool, Failure>> EndStreamAsync(
        EndStreamDto request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<bool, Failure>> ValidateStreamKeyAsync(
        string streamKey,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<bool, Failure>> StartStreamByKeyAsync(
        string streamKey,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<bool, Failure>> EndStreamByKeyAsync(
        string streamKey,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}
