using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Permission.PermissionRepository;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Contracts.Permissions;
using StreamingProject.Contracts.Permissions.PermissionDetailsDto;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Application.Service.Permission.PermissionService;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PermissionService> _logger;
    private readonly IValidator<AddPermissionDto> _addPermissionDtoValidator;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IMapper mapper, 
        ILogger<PermissionService> logger,
        IValidator<AddPermissionDto> addPermissionDtoValidator)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
        _addPermissionDtoValidator = addPermissionDtoValidator;
    }


    public async Task<Result<PermissionDetailsDto, Failure>> AddPermissionAsync(AddPermissionDto request, CancellationToken cancellationToken)
    {
        var validation = await _addPermissionDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return validation.ToErrors();

        var permission = PermissionEntity.Create(0, request.Name);
        
        var savedPermission = await _permissionRepository.AddPermissionAsync(permission);
        _logger.LogInformation("Permission {PermissionName} created",savedPermission.Name );
        
        var detailsDto = _mapper.Map<PermissionDetailsDto>(savedPermission);
        return Result.Success<PermissionDetailsDto, Failure>(detailsDto);
    }

    
    public async Task<Result<List<PermissionDetailsDto>, Failure>> GetPermissionsAsync(GetPermissionsDto request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetPermissionsAsync(request.UserId);

        return _mapper.Map<List<PermissionDetailsDto>>(permissions);      
    }

    
    public async Task<Result<bool, Failure>> RemovePermissionAsync(Guid userId, int permissionId, CancellationToken cancellationToken)
    {
        var success = await _permissionRepository.RemovePermissionAsync(userId, permissionId);

        if (!success)
            return Failure.FromError(Error.NotFound("Permission.NotFound", "User doesn't have this permission", null));
        
        _logger.LogInformation("Permission {PermissionId} removed from user {UserId}", permissionId, userId);
        return true;
    }
}