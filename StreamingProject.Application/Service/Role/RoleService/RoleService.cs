using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Contracts.Roles;
using StreamingProject.Contracts.Roles.RoleDetailsDto;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.Role.RoleService;

public class RoleService : IRoleService
{

    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;
    private readonly IValidator<AddRoleDto> _addRoleDtoValidator;


    public RoleService(
        IRoleRepository roleRepository, 
        IValidator<AddRoleDto> addRoleDtoValidator, 
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _addRoleDtoValidator = addRoleDtoValidator;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<RoleDetailsDto, Failure>> AddRoleAsync(AddRoleDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _addRoleDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var existingRole = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (existingRole != null)
            return Failure.FromError(Error.Validation("RoleAlreadyExists", "Role already exists", "RoleId"));
        
        var role = RoleEntity.Create(request.Id, request.Name);
        
        var result = await _roleRepository.AddRoleAsync(role);
        _logger.LogInformation("Role {RoleName} (ID: {RoleId} created",result.Name, result.Id);
        
        return _mapper.Map<RoleDetailsDto>(result);
    }


    public async Task<Result<RoleDetailsDto, Failure>> GetRoleByIdAsync(GetRoleByIdDto request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetRoleByIdAsync(request.Id);

        if (role == null)
            return Failure.FromError(Error.NotFound("Role.NotFound", $"Role with ID {request.Id} was not found", null));
        
        return _mapper.Map<RoleDetailsDto>(role);
    }

    public async Task<Result<RoleDetailsDto, Failure>> GetRoleByNameAsync(GetRoleByNameDto request, CancellationToken cancellationToken)
    {
        var role = _roleRepository.GetRoleByNameAsync(request.Name).Result;
        if (role == null)
            return Failure.FromError(Error.Validation("Role.NotFound", $"Role was not found", "RoleId"));
        
        return _mapper.Map<RoleDetailsDto>(role);
    }

    public async Task<Result<RoleDetailsDto, Failure>> UpdateRoleAsync(UpdateRoleDto request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (role == null) 
            return Failure.FromError(Error.Validation("RoleNotFound", "Role not found", "RoleId"));

        role.Name = request.Name;
        
        var result = await _roleRepository.UpdateRoleAsync(role);
        _logger.LogInformation("Role {RoleId} updated to {RoleName}", result.Id, result.Name);
        
        return _mapper.Map<RoleDetailsDto>(result);
    }

    public async Task<Result<bool, Failure>> DeleteRoleAsync(DeleteRoleDto request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetRoleByIdAsync(request.Id);
        if (role == null) 
            return Failure.FromError(Error.Validation("RoleNotFound", "Role not found", "RoleId"));
        
        var isDeleted = await _roleRepository.DeleteRoleAsync(request.Id);
        _logger.LogInformation("Role {RoleId} deleted", request.Id);

        return isDeleted;
    }
}