using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamValidators;

public class CreateStreamValidator : AbstractValidator<CreateStreamDto>
{
    public CreateStreamValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.StartDate).NotEmpty();
    }
}