using FluentValidation;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleValidators;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleDto>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
    }
}