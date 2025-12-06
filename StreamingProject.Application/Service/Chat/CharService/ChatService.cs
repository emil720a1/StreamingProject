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
    private readonly IValidator<ChatMessagesDto> _chatMessagesDtoValidator;
    private readonly IValidator<LeaveChatDto> _leaveChatDtoValidator;
    private readonly IValidator<DeleteMessageDto> _deleteMessageDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IChatRepository chatRepository, ILogger<ChatService> logger, IValidator<SendMessageDto> sendMessageDtoValidator, IMapper mapper, IValidator<ChatMessagesDto> chatMessagesDtoValidator, IValidator<LeaveChatDto> leaveChatDtoValidator, IValidator<DeleteMessageDto> deleteMessageDtoValidator)
    {
        _chatRepository = chatRepository;
        _sendMessageDtoValidator = sendMessageDtoValidator;
        _chatMessagesDtoValidator = chatMessagesDtoValidator;
        _leaveChatDtoValidator = leaveChatDtoValidator;
        _deleteMessageDtoValidator = deleteMessageDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<Result<ChatDetailsDto, Failure>> SendMessageAsync(SendMessageDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _sendMessageDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var messages = new ChatEntity
        {
            Id = request.Id,
            StreamId = request.StreamId,
            SentTime = request.SentAt,
            Message = request.Message,
            UserId = request.UserId
        };
        
        var result =  await _chatRepository.SendMessageAsync(messages); 
         
        var detailsDto = _mapper.Map<ChatDetailsDto>(result);

        return Result.Success < ChatDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<List<ChatDetailsDto>, Failure>> GetChatMessagesAsync(ChatMessagesDto request, CancellationToken cancellationToken)
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


    public async Task<Result<bool, Failure>> LeaveChatAsync(LeaveChatDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _leaveChatDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
           return validationResult.ToErrors();
        }

        var toLeaveChat = await _chatRepository.LeaveChatAsync(request.streamId);

        if (!toLeaveChat)
        {
            return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));
        }
        
        var result = Result.Success<bool, Failure>(toLeaveChat);
        
        _logger.LogInformation("User left chat");
        return result;
    }

    public async Task<Result<bool, Failure>> DeleteChatMessageAsync(DeleteMessageDto request, CancellationToken cancellationToken)
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
}