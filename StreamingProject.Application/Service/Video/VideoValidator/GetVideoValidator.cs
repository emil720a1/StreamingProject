using FluentValidation;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Application.Service.Video.VideoValidator;

public class GetVideoValidator : AbstractValidator<GetVideoDto>
{
    public GetVideoValidator()
    {
        RuleFor(x => x.VideoId)
            .NotEmpty().WithMessage("VideoId is required");

    }
}