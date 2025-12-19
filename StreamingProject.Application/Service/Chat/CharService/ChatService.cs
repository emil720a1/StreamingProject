using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Chat.ChatRepository;
using StreamingProject.Contracts.Chat;
using StreamingProject.Domain;
using StreamingProject.Domain.Chat;

namespace StreamingProject.Application.Service.Chat.CharService;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IValidator<SendMessageDto> _sendMessageDtoValidator;
    private readonly IValidator<GetChatMessagesDto> _chatMessagesDtoValidator;
    private readonly IValidator<DeleteMessageDto> _deleteMessageDtoValidator;
    private readonly IValidator<UpdateMessageDto> _updateMessageDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IChatRepository chatRepository, ILogger<ChatService> logger, IValidator<SendMessageDto> sendMessageDtoValidator, IMapper mapper, IValidator<GetChatMessagesDto> chatMessagesDtoValidator, IValidator<DeleteMessageDto> deleteMessageDtoValidator, IValidator<UpdateMessageDto> updateMessageDtoValidator)
    {
        _chatRepository = chatRepository;
        _sendMessageDtoValidator = sendMessageDtoValidator;
        _chatMessagesDtoValidator = chatMessagesDtoValidator;
        _deleteMessageDtoValidator = deleteMessageDtoValidator;
        _updateMessageDtoValidator = updateMessageDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<Result<ChatDetailsDto, Failure>> SendMessageAsync(SendMessageDto request, Guid UserId, CancellationToken cancellationToken)
    {
        var validationResult = await _sendMessageDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var messages = await _chatRepository.GetChatMessageById(request.StreamId);
        
        var result =  await _chatRepository.SendMessageAsync(messages); 
        var detailsDto = _mapper.Map<ChatDetailsDto>(result);

        return Result.Success < ChatDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<List<ChatDetailsDto>, Failure>> GetChatMessagesAsync(GetChatMessagesDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _chatMessagesDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var messages = await _chatRepository.GetChatMessagesAsync(request.StreamId);
        
        var detailsDto = _mapper.Map<List<ChatDetailsDto>>(messages);
        
        return Result.Success<List<ChatDetailsDto>, Failure>(detailsDto);
    }


  

    public async Task<Result<bool, Failure>> DeleteChatMessageAsync(DeleteMessageDto request, Guid UserId, CancellationToken cancellationToken)
    {
        
        var validationResult = await _deleteMessageDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var toDeleteChat = await _chatRepository.DeleteChatMessageAsync(request.Id);


        if (!toDeleteChat)
        {
            return Failure.FromError(Error.Validation("MessageNotFound", "Message not found", "MessageId"));
        }
        
        
        var result = Result.Success<bool, Failure>(toDeleteChat);
        
        _logger.LogInformation("Message deleted");
        return result;
    }

    public async Task<Result<bool, Failure>> UpdateChatMessageAsync(UpdateMessageDto request, Guid userId, CancellationToken cancellationToken)
    {
        var validationResult = await _updateMessageDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var chatEntity = await _chatRepository.GetChatMessageById(request.streamId);

        if (chatEntity is null)
        {
            return Failure.FromError(Error.Validation("MessageNotFound", "Message not found", "MessageId"));
        }

        if (chatEntity.UserId != userId)
        {
            return Failure.FromError(Error.Unauthorized("Unauthorized", "Unauthorized"));
        }

        chatEntity.Message = request.newText;
        
        var toUpdateChat = await _chatRepository.UpdateChatMessageAsync(chatEntity);

        if (!toUpdateChat)
        {
            return Failure.FromError(Error.Validation("MessageNotFound", "Message not found", "MessageId"));
        }
        
        var result = Result.Success<bool, Failure>(toUpdateChat);
        
        _logger.LogInformation("Message updated");
        return result;
    }
}