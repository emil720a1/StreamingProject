using FluentValidation;
using StreamingProject.Contracts.Permissions;

namespace StreamingProject.Application.Service.Permission.PermissionValidators;

public class GetPermissionValidator : AbstractValidator<GetPermissionsDto>
{
    public GetPermissionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}