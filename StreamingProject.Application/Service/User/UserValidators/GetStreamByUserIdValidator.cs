using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.User.UserValidators;

public class GetStreamByUserIdValidator : AbstractValidator<GetStreamByIdDto>
{
    public GetStreamByUserIdValidator()
    {
        RuleFor(x => x.streamId).NotEmpty().WithMessage("StreamId is required");
    }
}