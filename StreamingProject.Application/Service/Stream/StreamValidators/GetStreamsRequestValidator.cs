using FluentValidation;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.Application.Service.Stream.StreamValidators;

public sealed class GetStreamsRequestValidator : AbstractValidator<GetStreamsRequestDto>
{
    private static readonly string[] AllowedSortFields = ["startTime", "viewerCount"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetStreamsRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.SortBy)
            .NotEmpty()
            .Must(value => AllowedSortFields.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortBy must be either 'startTime' or 'viewerCount'.");

        RuleFor(request => request.SortDirection)
            .NotEmpty()
            .Must(value => AllowedSortDirections.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be either 'asc' or 'desc'.");
    }
}
