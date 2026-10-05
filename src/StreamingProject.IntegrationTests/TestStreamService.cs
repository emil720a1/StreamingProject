using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.IntegrationTests;

public sealed class TestStreamService : IStreamService
{
    public Task<Result<List<StreamListItemDto>, Failure>> GetAvailableStreamsAsync(
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
                DateTime.UtcNow)
        };

        return Task.FromResult(
            Result.Success<List<StreamListItemDto>, Failure>(streams));
    }

    public Task<Result<StreamDetailsDto, Failure>> CreateStreamAsync(
        CreateStreamDto streamDto,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<Result<StreamDetailsDto, Failure>> JoinStreamAsync(
        JoinStreamDto streamDto,
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
}
