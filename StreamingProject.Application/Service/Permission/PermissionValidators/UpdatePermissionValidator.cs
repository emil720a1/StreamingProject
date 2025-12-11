using FluentValidation;
using StreamingProject.Contracts.Permissions;

namespace StreamingProject.Application.Service.Permission.PermissionValidators;

public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionDto>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Name).NotEmpty();
        
        RuleFor(x => x.Permissions).NotEmpty();
        
        
    }
}