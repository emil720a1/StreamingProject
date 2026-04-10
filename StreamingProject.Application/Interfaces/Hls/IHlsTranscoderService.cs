using Shared;
using CSharpFunctionalExtensions;

namespace StreamingProject.Application.Interfaces.Hls;

public interface IHlsTranscoderService
{
    Task<Result<bool, Failure>> StartTranscodingAsync(string streamKey, string outputDirectory, CancellationToken cancellationToken = default);
    Task<Result<bool, Failure>> StopTranscodingAsync(string streamKey, CancellationToken cancellationToken = default);
    Task<Result<string, Failure>> GetHlsPlaylistUrlAsync(string streamId, CancellationToken cancellationToken = default);
}
