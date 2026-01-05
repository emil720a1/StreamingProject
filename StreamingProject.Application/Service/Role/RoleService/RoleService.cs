using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Contracts.Roles;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.Role.RoleService;

public class RoleService : IRoleService
{

    private readonly IRoleRepository _roleRepository;
    private readonly IValidator<AddRoleDto> _addRoleDtoValidator;
    private readonly IValidator<GetRoleByNameDto> _getRoleByNameDtoValidator;
    private readonly IValidator<GetRoleByIdDto> _getRoleByIdDtoValidator;
    private readonly IValidator<UpdateRoleDto> _updateRoleDtoValidator;
    private readonly IValidator<DeleteRoleDto> _deleteRoleDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;


    public RoleService(IRoleRepository roleRepository, IValidator<AddRoleDto> addRoleDtoValidator, IValidator<GetRoleByNameDto> getRoleByNameDtoValidator, IValidator<GetRoleByIdDto> getRoleByIdDtoValidator, IValidator<UpdateRoleDto> updateRoleDtoValidator, IValidator<DeleteRoleDto> deleteRoleDtoValidator, IMapper mapper, ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _addRoleDtoValidator = addRoleDtoValidator;
        _getRoleByNameDtoValidator = getRoleByNameDtoValidator;
        _getRoleByIdDtoValidator = getRoleByIdDtoValidator;
        _updateRoleDtoValidator = updateRoleDtoValidator;
        _deleteRoleDtoValidator = deleteRoleDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<RoleDetailsDto, Failure>> AddRoleAsync(AddRoleDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _addRoleDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var existingRole = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (existingRole != null)
        {
            return Failure.FromError(Error.Validation("RoleAlreadyExists", "Role already exists", "RoleId"));
        }
        
        
        var role = RoleEntity.Create(request.Id, request.Name);
        
        var result = await _roleRepository.AddRoleAsync(role);
        
        _logger.LogInformation("Role {RoleId} created", result.Id);
        
        var detailsDto = _mapper.Map<RoleDetailsDto>(result);

        return Result.Success<RoleDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<RoleDetailsDto, Failure>> GetRoleByName(GetRoleByNameDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getRoleByNameDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var result = await _roleRepository.GetRoleByNameAsync(request.Name);
        
        _logger.LogInformation("Role retrieved");
        
        var detailsDto = _mapper.Map<RoleDetailsDto>(result);

        return Result.Success<RoleDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<RoleDetailsDto, Failure>> GetRoleById(GetRoleByIdDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getRoleByIdDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        
        var result = await _roleRepository.GetRoleByIdAsync(request.Id);
        
        _logger.LogInformation("Role retrieved");
        
        var detailsDto = _mapper.Map<RoleDetailsDto>(result);
        
        return Result.Success<RoleDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<RoleDetailsDto, Failure>> UpdateRoleAsync(UpdateRoleDto request, CancellationToken cancellationToken)
    {
        var validationResult = await  _updateRoleDtoValidator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var role = await _roleRepository.GetRoleByIdAsync(request.Id);

        if (role == null) return Failure.FromError(Error.Validation("RoleNotFound", "Role not found", "RoleId"));

        role.Name = request.Name;
        
        var result = await _roleRepository.UpdateRoleAsync(role);
        
        _logger.LogInformation("Role {RoleId} updated", result.Id);
        
        var detailsDto = _mapper.Map<RoleDetailsDto>(result);
        
        return Result.Success<RoleDetailsDto, Failure>(detailsDto);

    }

    public async Task<Result<RoleDetailsDto, Failure>> DeleteRoleAsync(DeleteRoleDto request,
        CancellationToken cancellationToken)
    {
        var validateResult = await _deleteRoleDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validateResult.IsValid)
        {
            return validateResult.ToErrors();
        }
        
        var role = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (role == null) return Failure.FromError(Error.Validation("RoleNotFound", "Role not found", "RoleId"));
        
        var detailsDto = _mapper.Map<RoleDetailsDto>(role);
        
        
        var isDeleted = await _roleRepository.DeleteRoleAsync(request.Id);
        
        _logger.LogInformation("Role {RoleId} deleted", request.Id);
        
        return Result.Success<RoleDetailsDto, Failure>(detailsDto);
    }
}