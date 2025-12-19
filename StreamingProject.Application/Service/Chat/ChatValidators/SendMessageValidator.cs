using FluentValidation;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class SendMessageValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.StreamId).NotEmpty();
        
        RuleFor(x => x.Message).NotEmpty();
    }
}