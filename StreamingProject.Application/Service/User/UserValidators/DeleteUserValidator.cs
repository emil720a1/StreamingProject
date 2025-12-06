using FluentValidation;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Service.User.UserValidators;

public class DeleteUserValidator : AbstractValidator<DeleteUserDto>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
    }
}