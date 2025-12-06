using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.CharService;

public interface IChatService
{
    Task<Result<ChatDetailsDto, Failure>> SendMessageAsync(SendMessageDto sendMessageDto, CancellationToken cancellationToken);
    
    Task<Result<List<ChatDetailsDto>, Failure>> GetChatMessagesAsync(ChatMessagesDto chatMessagesDto, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> LeaveChatAsync(LeaveChatDto leaveChatDto, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> DeleteChatMessageAsync(DeleteMessageDto deleteMessageDto, CancellationToken cancellationToken);
}