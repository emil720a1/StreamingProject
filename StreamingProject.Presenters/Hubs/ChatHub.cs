using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace StreamingProject.Presenters.Hubs;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "SignalR client connected {ConnectionId}",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is null)
        {
            _logger.LogInformation(
                "SignalR client disconnected {ConnectionId}",
                Context.ConnectionId);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "SignalR client disconnected with error {ConnectionId}",
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinStreamGroup(string streamId)
    {
        try
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                streamId);

            _logger.LogInformation(
                "SignalR client joined stream group {ConnectionId} {StreamId}",
                Context.ConnectionId,
                streamId);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to join SignalR stream group {ConnectionId} {StreamId}",
                Context.ConnectionId,
                streamId);

            throw;
        }
    }

    public async Task LeaveStreamGroup(string streamId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                streamId);

            _logger.LogInformation(
                "SignalR client left stream group {ConnectionId} {StreamId}",
                Context.ConnectionId,
                streamId);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to leave SignalR stream group {ConnectionId} {StreamId}",
                Context.ConnectionId,
                streamId);

            throw;
        }
    }
}
