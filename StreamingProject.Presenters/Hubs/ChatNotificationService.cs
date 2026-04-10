using Microsoft.AspNetCore.SignalR;
using StreamingProject.Application.Interfaces.Chat;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Presenters.Hubs;

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastMessageAsync(string streamId, ChatDetailsDto message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(streamId).SendAsync("ReceiveMessage", message, cancellationToken);
    }

    public async Task NotifyMessageDeletedAsync(string streamId, Guid messageId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(streamId).SendAsync("MessageDeleted", messageId, cancellationToken);
    }

    public async Task NotifyMessageUpdatedAsync(string streamId, ChatDetailsDto message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(streamId).SendAsync("MessageUpdated", message, cancellationToken);
    }
}
