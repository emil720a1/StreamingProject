using FluentValidation;
using StreamingProject.Contracts.Permissions;

namespace StreamingProject.Application.Service.Permission.PermissionValidators;

public class AddPermissionValidator : AbstractValidator<AddPermissionDto>
{
    public AddPermissionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.Permission).NotEmpty();
    }
}