using FluentValidation;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application.Validators;

public class GetStreamsValidator : AbstractValidator<GetStreamByIdDto>
{
    public GetStreamsValidator()
    {
        RuleFor(x => x.streamId).NotEmpty().WithMessage("StreamId is required");
    }
}