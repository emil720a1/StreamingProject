using FluentValidation;
using StreamingProject.Contracts.VideoDto.Crud;

namespace StreamingProject.Application.Service.Video.VideoValidator;

public class DeleteVideoValidator : AbstractValidator<DeleteVideoDto>
{
    public DeleteVideoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}