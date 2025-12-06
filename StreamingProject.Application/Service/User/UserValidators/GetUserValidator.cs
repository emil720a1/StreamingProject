using FluentValidation;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Service.User.UserValidators;

public class GetUserValidator : AbstractValidator<GetUserDto>
{
    public GetUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}