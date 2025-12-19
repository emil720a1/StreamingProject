using FluentValidation;
using StreamingProject.Contracts.Chat;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class GetChatMessagesValidator : AbstractValidator<GetChatMessagesDto>
{
    public GetChatMessagesValidator()
    {
        RuleFor(x => x.StreamId).NotEmpty();
        
    }
}