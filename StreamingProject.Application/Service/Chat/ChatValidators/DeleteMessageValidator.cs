using FluentValidation;
using StreamingProject.Contracts.Chat;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class DeleteMessageValidator : AbstractValidator<DeleteMessageDto>
{
    public DeleteMessageValidator()
    {
        RuleFor(x => x.StreamId).NotEmpty();
        
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.Message).NotEmpty();
        
    }
}