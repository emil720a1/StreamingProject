using CSharpFunctionalExtensions;
using Shared;
using StreamingProject.Contracts.Permissions;
using StreamingProject.Contracts.Permissions.PermissionDetailsDto;
using StreamingProject.Domain.Enums;

namespace StreamingProject.Application.Service.Permission.PermissionService;

public interface IPermissionService
{
    Task<Result<PermissionDetailsDto, Failure>> AddPermissionAsync(AddPermissionDto request, CancellationToken cancellationToken);
    
    Task<Result<List<PermissionDetailsDto>, Failure>> GetPermissionsAsync(GetPermissionsDto request, CancellationToken cancellationToken);
    Task<Result<bool, Failure>> UpdatePermissionAsync(UpdatePermissionDto request, CancellationToken cancellationToken);
    Task<Result<bool, Failure>> RemovePermissionAsync(RemovePermissionDto request, CancellationToken cancellationToken);
    
}