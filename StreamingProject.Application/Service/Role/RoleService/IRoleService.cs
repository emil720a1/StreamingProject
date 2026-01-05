using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Roles;

namespace StreamingProject.Application.Service.Role.RoleService;

public interface IRoleService
{
    
    Task<Result<RoleDetailsDto, Failure>> AddRoleAsync(AddRoleDto request, CancellationToken cancellationToken);

    Task<Result<RoleDetailsDto, Failure>> GetRoleByName(GetRoleByNameDto request, CancellationToken cancellationToken);

    Task<Result<RoleDetailsDto, Failure>> GetRoleById(GetRoleByIdDto request, CancellationToken cancellationToken);

    Task<Result<RoleDetailsDto, Failure>> UpdateRoleAsync(UpdateRoleDto request, CancellationToken cancellationToken);
    
    Task<Result<RoleDetailsDto, Failure>> DeleteRoleAsync(DeleteRoleDto request, CancellationToken cancellationToken);
    
    
}