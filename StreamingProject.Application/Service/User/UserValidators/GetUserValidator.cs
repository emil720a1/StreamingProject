using FluentValidation;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Validators;

public class GetUserValidator : AbstractValidator<GetUserDto>
{
    public GetUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}