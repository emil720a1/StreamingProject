using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.CharService;

public interface IChatService
{
    Task<Result<ChatDetailsDto, Failure>> SendMessageAsync(SendMessageDto sendMessageDto, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<List<ChatDetailsDto>, Failure>> GetChatMessagesAsync(GetChatMessagesDto getChatMessagesDto, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> DeleteChatMessageAsync(DeleteMessageDto deleteMessageDto, Guid userId, CancellationToken cancellationToken);

    Task<Result<bool, Failure>> UpdateChatMessageAsync(UpdateMessageDto updateMessageDto, Guid userId,
        CancellationToken cancellationToken);
}