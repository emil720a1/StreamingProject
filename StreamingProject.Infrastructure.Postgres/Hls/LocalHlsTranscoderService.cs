using System.Collections.Concurrent;
using System.Diagnostics;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Shared;
using StreamingProject.Application.Interfaces.Hls;

namespace StreamingProject.Infrastructure.Postgres.Hls;

public class LocalHlsTranscoderService : IHlsTranscoderService
{
    private readonly ILogger<LocalHlsTranscoderService> _logger;
    private readonly ConcurrentDictionary<string, Process> _activeProcesses = new();

    public LocalHlsTranscoderService(ILogger<LocalHlsTranscoderService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<bool, Failure>> StartTranscodingAsync(string streamKey, string outputDirectory, CancellationToken cancellationToken = default)
    {
        if (_activeProcesses.ContainsKey(streamKey))
        {
            return Failure.FromError(Error.Conflict("Hls.AlreadyRunning", "Transcoding is already running for this stream."));
        }

        try
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var playlistPath = Path.Combine(outputDirectory, "index.m3u8");

            // Example FFmpeg command for RTMP to HLS
            // This assumes RTMP input is being pushed to something like rtmp://localhost/live/{streamKey}
            // For a complete local simulation, you would run an RTMP ingest server (e.g., NGINX RTMP).
            // Here we just prepare the command line to read from a hypothetical RTMP URL.
            var rtmpUrl = $"rtmp://localhost/live/{streamKey}";
            
            var arguments = $"-i {rtmpUrl} -c:v libx264 -c:a aac -f hls -hls_time 4 -hls_playlist_type event -hls_flags independent_segments -hls_segment_filename \"{Path.Combine(outputDirectory, "%03d.ts")}\" \"{playlistPath}\"";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = processStartInfo };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    _logger.LogDebug("FFmpeg: {Data}", e.Data);
                }
            };

            process.Start();
            process.BeginErrorReadLine();

            _activeProcesses.TryAdd(streamKey, process);

            _logger.LogInformation("Started FFmpeg transcoding for stream {StreamKey}", streamKey);
            
            return Result.Success<bool, Failure>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start FFmpeg for stream {StreamKey}", streamKey);
            return Failure.FromError(Error.Failure("Hls.StartFailed", ex.Message));
        }
    }

    public Task<Result<bool, Failure>> StopTranscodingAsync(string streamKey, CancellationToken cancellationToken = default)
    {
        if (_activeProcesses.TryRemove(streamKey, out var process))
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(true);
                }
                
                process.Dispose();
                _logger.LogInformation("Stopped FFmpeg transcoding for stream {StreamKey}", streamKey);
                return Task.FromResult(Result.Success<bool, Failure>(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping FFmpeg for stream {StreamKey}", streamKey);
                return Task.FromResult(Result.Failure<bool, Failure>(Failure.FromError(Error.Failure("Hls.StopFailed", ex.Message))));
            }
        }

        return Task.FromResult(Result.Failure<bool, Failure>(Failure.FromError(Error.NotFound("Hls.NotFound", "No running process found for this stream.", null))));
    }

    public Task<Result<string, Failure>> GetHlsPlaylistUrlAsync(string streamId, CancellationToken cancellationToken = default)
    {
        // Generate a URL based on standard structure (e.g. /hls/{streamId}/index.m3u8)
        var url = $"/hls/{streamId}/index.m3u8";
        return Task.FromResult(Result.Success<string, Failure>(url));
    }
}
