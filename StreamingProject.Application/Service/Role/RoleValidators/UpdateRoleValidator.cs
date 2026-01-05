using System.Data;
using FluentValidation;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleValidators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.Name).NotEmpty();
        
    }
}