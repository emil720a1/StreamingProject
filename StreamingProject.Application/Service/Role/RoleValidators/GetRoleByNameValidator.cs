using FluentValidation;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleValidators;

public class GetRoleByNameValidator : AbstractValidator<GetRoleByNameDto>
{
    public GetRoleByNameValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        
    }
}