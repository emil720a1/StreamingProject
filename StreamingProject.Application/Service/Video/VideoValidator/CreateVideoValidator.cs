using FluentValidation;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Application.Service.Video.VideoValidator;

public class CreateVideoValidator : AbstractValidator<CreateVideoDto>
{
    public CreateVideoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(50).WithMessage("Title must not exceed 50 characters");
        
        RuleFor(x => x.StreamId)
            .NotEmpty().WithMessage("StreamId is required");
        
    }
}