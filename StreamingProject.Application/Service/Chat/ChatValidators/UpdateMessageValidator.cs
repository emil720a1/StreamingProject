using FluentValidation;
using StreamingProject.Contracts.Chat;
using StreamingProject.Contracts.Chat.CrudDtos;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class UpdateMessageValidator: AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageValidator()
    {
        RuleFor(a => a.MessageId)
            .NotEmpty()
            .WithMessage("UserId is required");
        
        RuleFor(a => a.MessageId)
            .NotEmpty()
            .WithMessage("MessageId is required");
        
    }
}