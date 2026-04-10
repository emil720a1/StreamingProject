using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Chat.ChatRepository;
using StreamingProject.Contracts.Chat;
using StreamingProject.Contracts.Chat.CrudDtos;
using StreamingProject.Domain;
using StreamingProject.Domain.Chat;
using StreamingProject.Application.Interfaces.Chat;

namespace StreamingProject.Application.Service.Chat.CharService;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatService> _logger;
    private readonly IValidator<SendMessageDto> _sendMessageDtoValidator;
    private readonly IChatNotificationService _chatNotificationService;
    
    public ChatService(
        IChatRepository chatRepository, 
        ILogger<ChatService> logger, 
        IMapper mapper,
        IValidator<SendMessageDto> sendMessageDtoValidator,
        IChatNotificationService chatNotificationService)
    {
        _chatRepository = chatRepository;
        _sendMessageDtoValidator = sendMessageDtoValidator;
        _mapper = mapper;
        _logger = logger;
        _chatNotificationService = chatNotificationService;
    }


    public async Task<Result<ChatDetailsDto, Failure>> SendMessageAsync(SendMessageDto request, Guid userId, CancellationToken cancellationToken)
    {
        var validationResult = await _sendMessageDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var messageEntity = ChatEntity.Create(userId, request.StreamId, request.Message);
        
        var result =  await _chatRepository.SendMessageAsync(messageEntity); 
        
        _logger.LogInformation("User {UserId} sent message to stream {StreamId}", userId, request.StreamId);
        
        var chatDetailsDto = _mapper.Map<ChatDetailsDto>(result);
        await _chatNotificationService.BroadcastMessageAsync(request.StreamId.ToString(), chatDetailsDto, cancellationToken);
        
        return chatDetailsDto;
    }

    public async Task<Result<List<ChatDetailsDto>, Failure>> GetChatMessagesAsync(GetChatMessagesDto request, CancellationToken cancellationToken)
    {
        var messages = await _chatRepository.GetChatMessagesAsync(request.StreamId);
        
        return _mapper.Map<List<ChatDetailsDto>>(messages);
    }

    public async Task<Result<bool, Failure>> DeleteChatMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken)
    {
        var message = await _chatRepository.GetChatMessageById(messageId);

        if (message == null)
            return Failure.FromError(Error.NotFound("Chat.MessageNotFound", "Message not found", null));
        
        if (message.UserId != userId)
            return Failure.FromError(Error.Unauthorized( "Chat.Unauthorized", "You cannot delete someone else's message"));
        
        var success = await _chatRepository.DeleteChatMessageAsync(messageId);
        _logger.LogInformation("Message {MessageId} deleted by user {UserId}",  messageId, userId);
        
        if (success)
        {
            await _chatNotificationService.NotifyMessageDeletedAsync(message.StreamId.ToString(), messageId, cancellationToken);
        }
        
        return success;
    }

    public async Task<Result<bool, Failure>> UpdateChatMessageAsync(UpdateMessageDto request, Guid userId, CancellationToken cancellationToken)
    {
        var message = await _chatRepository.GetChatMessageById(request.MessageId);
        
        if (message == null)
            return Failure.FromError(Error.NotFound("Chat.MessageNotFound", "Message not found", null));

        if (message.UserId != userId)
        {
            _logger.LogWarning("User {UserId} tried to edit message {MessageId} owned by {OwnerId}", userId, message.Id, message.UserId);
            return Failure.FromError(Error.Unauthorized("Chat.Unauthorized", "You cannot edit someone else's message"));
        }
        
        message.UpdateText(request.NewText);
        
        var success = await _chatRepository.UpdateChatMessageAsync(message);
        _logger.LogInformation("Message {MessageId} updated by user {UserId}", message.Id, userId);
        
        if (success)
        {
            var chatDetailsDto = _mapper.Map<ChatDetailsDto>(message);
            await _chatNotificationService.NotifyMessageUpdatedAsync(message.StreamId.ToString(), chatDetailsDto, cancellationToken);
        }
        
        return success;
    }
}