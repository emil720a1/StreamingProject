using LiveStreamingServerNet;
using LiveStreamingServerNet.Networking.Server.Contracts;
using LiveStreamingServerNet.Rtmp.Server.Contracts;
using LiveStreamingServerNet.Utilities;
using LiveStreamingServerNet.Utilities.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamingProject.Application.Service.Stream.StreamService;

namespace StreamingProject.Presenters.Handlers;

public class RtmpServerEventHandler : IRtmpServerStreamEventHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RtmpServerEventHandler> _logger;
    private readonly IServerHandle _server;

    public RtmpServerEventHandler(ILogger<RtmpServerEventHandler> logger, IServiceScopeFactory scopeFactory, IServerHandle server)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _server = server;
    }


    public async ValueTask OnRtmpStreamPublishedAsync(
        IEventContext context, 
        uint clientId, 
        string streamPath,
        IReadOnlyDictionary<string, string> streamArguments)
    {
        var streamKey = streamPath.Replace("/live/", "");
        
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetService<IStreamService>();

        var result = await service.ValidateStreamKeyAsync(streamKey, default);

        if (result.IsFailure)
        {
            var client = _server.Clients.FirstOrDefault(c => c.Id == clientId);
            client?.Disconnect();
            
            _logger.LogWarning("Клієнта відключено через невірний ключ");
            return;
        }
        
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "live", streamKey);
        Directory.CreateDirectory(outputDir);

        var ffmpegArgs =
            $"-i rtmp://localhost:1935/live/{streamKey} -c:v copy -c:a copy -f hls -hls_time 2 -hls_list_size 3 -hls_flags delete_segments {outputDir}/index.m3u8";

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = ffmpegArgs,
            CreateNoWindow = true,
            UseShellExecute = false,
        });
        
        _logger.LogInformation("Стрім опубліковано! Клієнт: {ClientId}, Шлях: {StreamPath}", clientId, streamPath);
    }

    public ValueTask OnRtmpStreamUnpublishedAsync(IEventContext context, uint clientId, string streamPath)
    {
        _logger.LogInformation("Стрім зупинено. Клієнт: {ClientId}, Шлях: {StreamPath}", clientId, streamPath);
        return ValueTask.CompletedTask;
    }

    public ValueTask OnRtmpStreamSubscribedAsync(IEventContext context, uint clientId, string streamPath, 
        IReadOnlyDictionary<string, string> streamArguments) => ValueTask.CompletedTask;

    public ValueTask OnRtmpStreamUnsubscribedAsync(IEventContext context, uint clientId, string streamPath) => ValueTask.CompletedTask;

    public ValueTask OnRtmpStreamMetaDataReceivedAsync(IEventContext context, uint clientId, string streamPath,
        IReadOnlyDictionary<string, object> metaData) => ValueTask.CompletedTask;
    
    
    
    
    
    
    
}