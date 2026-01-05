using FluentValidation;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleValidators;

public class AddRoleValidator : AbstractValidator<AddRoleDto>
{
    public AddRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(x => x.Permissions).NotEmpty();

        RuleFor(x => x.Id).NotEmpty();
    }
}