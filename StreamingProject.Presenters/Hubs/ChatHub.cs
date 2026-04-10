using Microsoft.AspNetCore.SignalR;

namespace StreamingProject.Presenters.Hubs;

public class ChatHub : Hub
{
    public async Task JoinStreamGroup(string streamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, streamId);
    }

    public async Task LeaveStreamGroup(string streamId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, streamId);
    }
}
