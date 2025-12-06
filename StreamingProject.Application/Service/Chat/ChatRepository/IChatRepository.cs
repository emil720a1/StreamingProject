using StreamingProject.Domain;
using StreamingProject.Domain.Chat;

namespace StreamingProject.Application.Service.Chat.ChatRepository;

public interface IChatRepository
{
    
    Task<ChatEntity> SendMessageAsync(ChatEntity message);
    Task<List<ChatEntity>> GetChatMessagesAsync(Guid streamId);
    Task<bool> LeaveChatAsync(Guid streamId);
    
    Task<bool> DeleteChatMessageAsync(Guid messageId);
    
}