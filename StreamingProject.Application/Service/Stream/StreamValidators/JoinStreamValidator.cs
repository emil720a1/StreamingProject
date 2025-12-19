using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamValidators;

public class JoinStreamValidator : AbstractValidator<JoinStreamDto>
{
    public JoinStreamValidator()
    {
        
        RuleFor(x => x.UserId).NotEmpty();
        
        
        RuleFor(x => x.StreamId).NotEmpty();
    }
}