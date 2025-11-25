using FluentValidation;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class LeaveMessageValidator : AbstractValidator<LeaveChatDto>
{
    public LeaveMessageValidator()
    {
        RuleFor(x => x.streamId).NotEmpty();
        
        RuleFor(x => x.userId).NotEmpty();
    }
}