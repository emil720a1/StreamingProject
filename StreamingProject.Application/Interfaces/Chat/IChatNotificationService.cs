using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Interfaces.Chat;

public interface IChatNotificationService
{
    Task BroadcastMessageAsync(string streamId, ChatDetailsDto message, CancellationToken cancellationToken = default);
    Task NotifyMessageDeletedAsync(string streamId, Guid messageId, CancellationToken cancellationToken = default);
    Task NotifyMessageUpdatedAsync(string streamId, ChatDetailsDto message, CancellationToken cancellationToken = default);
}
