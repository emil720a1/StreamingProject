using FluentValidation;
using StreamingProject.Contracts.Chat;
using StreamingProject.Contracts.Chat.CrudDtos;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Service.Chat.ChatValidators;

public class DeleteMessageValidator : AbstractValidator<DeleteMessageDto>
{
    public DeleteMessageValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
    }
}