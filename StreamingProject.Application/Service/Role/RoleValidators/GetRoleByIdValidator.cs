using FluentValidation;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleValidators;

public class GetRoleByIdValidator : AbstractValidator<GetRoleByIdDto>
{
    public GetRoleByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}