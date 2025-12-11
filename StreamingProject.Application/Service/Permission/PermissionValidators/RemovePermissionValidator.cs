using FluentValidation;
using StreamingProject.Contracts.Permissions;

namespace StreamingProject.Application.Service.Permission.PermissionValidators;

public class RemovePermissionValidator : AbstractValidator<RemovePermissionDto>
{
    public RemovePermissionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Permission).NotEmpty();
    }
}