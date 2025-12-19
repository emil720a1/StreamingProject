using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamValidators;

public class GetStreamValidator : AbstractValidator<GetStreamByIdDto>
{
    public GetStreamValidator()
    {
        RuleFor(x => x.streamId).NotEmpty();
        
        RuleFor(x => x.UserId).NotEmpty();
        
    }
}