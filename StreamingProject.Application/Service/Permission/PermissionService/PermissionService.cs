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
    private readonly IValidator<AddPermissionDto> _addPermissionDtoValidator;
    private readonly IValidator<GetPermissionsDto> _getPermissionDtoValidator;
    private readonly IValidator<UpdatePermissionDto> _updatePermissionDtoValidator;
    private readonly IValidator<RemovePermissionDto> _removePermissionDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(IUserRepository usersRepository, IValidator<AddPermissionDto> addPermissionDtoValidator, IValidator<RemovePermissionDto> removePermissionDtoValidator, IValidator<GetPermissionsDto> getPermissionDtoValidator, IValidator<UpdatePermissionDto> updatePermissionDtoValidator, IMapper mapper, ILogger<PermissionService> logger, IPermissionRepository permissionRepository, IValidator<RemovePermissionDto> removePermissionDtoValidator1)
    {
        _addPermissionDtoValidator = addPermissionDtoValidator;
        _getPermissionDtoValidator = getPermissionDtoValidator;
        _removePermissionDtoValidator = removePermissionDtoValidator1;
        _updatePermissionDtoValidator = updatePermissionDtoValidator;
        _mapper = mapper;
        _logger = logger;
        _permissionRepository = permissionRepository;
    }


    public async Task<Result<PermissionDetailsDto, Failure>> AddPermissionAsync(AddPermissionDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _addPermissionDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var permissions = new PermissionEntity
        {
            Id = Guid.NewGuid().ToByteArray().GetHashCode(),
            Name = request.Name
        };
        
        
        var savePermissions = await _permissionRepository.AddPermissionAsync(permissions.Id, permissions);
        
        return _mapper.Map<PermissionDetailsDto>(savePermissions);
    }

    
    

    public async Task<Result<List<PermissionDetailsDto>, Failure>> GetPermissionsAsync(GetPermissionsDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getPermissionDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var permissions = await _permissionRepository.GetPermissionsAsync(request.UserId);

        var details = _mapper.Map<List<PermissionDetailsDto>>(permissions);
        
        return Result.Success<List<PermissionDetailsDto>, Failure>(details);
    }

    public async Task<Result<bool, Failure>> UpdatePermissionAsync(UpdatePermissionDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _updatePermissionDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var permission = await _permissionRepository.GetPermissionAsync(request.UserId);
        
        var savePermissions = await _permissionRepository.UpdatePermissionAsync(permission.Id, permission);
        
        
        var result = _mapper.Map<bool>(savePermissions);
        
        _logger.LogInformation("Permission updated");
        
        
        return result;
    }
    
    
    public async Task<Result<bool, Failure>> RemovePermissionAsync(RemovePermissionDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _removePermissionDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var permission = await _permissionRepository.GetPermissionAsync(request.UserId);
        
        var toDeletePermissions = await _permissionRepository.RemovePermissionAsync(permission.Id, permission );

        if (!toDeletePermissions)
        {
            return Failure.FromError(Error.Validation("MessageNotFound", "message not found", "messageId"));
        }
        
        var result = Result.Success<bool, Failure>(toDeletePermissions);
        
        _logger.LogInformation("Permission Removed");
        
        return result;
    }
}