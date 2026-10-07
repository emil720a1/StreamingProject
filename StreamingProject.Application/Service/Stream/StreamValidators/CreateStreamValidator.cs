using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamValidators;

public class CreateStreamValidator : AbstractValidator<CreateStreamDto>
{
    public CreateStreamValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2048)
            .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl));
    }
}
