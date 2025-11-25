using CSharpFunctionalExtensions;
using StreamingProject.Contracts.Chat;
using StreamingProject.Domain;

namespace StreamingProject.Application;

public interface IChatRepository
{
    
    Task<ChatEntity> SendMessageAsync(ChatEntity message);
    Task<List<ChatEntity>> GetChatMessagesAsync(Guid streamId);
    Task<bool> LeaveChatAsync(Guid streamId);
    
    Task<bool> DeleteChatMessageAsync(Guid messageId);
    
}