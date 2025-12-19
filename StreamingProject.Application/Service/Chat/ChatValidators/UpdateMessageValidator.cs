using FluentValidation;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class UpdateMessageValidator: AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageValidator()
    {
        RuleFor(a => a.userId)
            .NotEmpty()
            .WithMessage("UserId is required");
        
        RuleFor(a => a.messageId)
            .NotEmpty()
            .WithMessage("MessageId is required");
        
    }
}