using FluentValidation;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class ChatMessagesValidator : AbstractValidator<ChatMessagesDto>
{
    public ChatMessagesValidator()
    {
        RuleFor(x => x.StreamId).NotEmpty();
        
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Message).NotEmpty();
        
        RuleFor(x => x.SentAt).NotEmpty();
        
    }
}