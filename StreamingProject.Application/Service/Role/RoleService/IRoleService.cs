using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Roles;
using StreamingProject.Contracts.Roles.RoleDetailsDto;

namespace StreamingProject.Application.Service.Role.RoleService;

public interface IRoleService
{
    
    Task<Result<RoleDetailsDto, Failure>> AddRoleAsync(AddRoleDto request, CancellationToken cancellationToken);

    Task<Result<RoleDetailsDto, Failure>> GetRoleByIdAsync(GetRoleByIdDto request, CancellationToken cancellationToken);
    
    Task<Result<RoleDetailsDto, Failure>> GetRoleByNameAsync(GetRoleByNameDto request, CancellationToken cancellationToken);

    Task<Result<RoleDetailsDto, Failure>> UpdateRoleAsync(UpdateRoleDto request, CancellationToken cancellationToken);
    
    Task<Result<bool, Failure>> DeleteRoleAsync(DeleteRoleDto request, CancellationToken cancellationToken);
    
    
}