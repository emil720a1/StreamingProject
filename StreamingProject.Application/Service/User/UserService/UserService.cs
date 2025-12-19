using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.User.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IValidator<AddUserDto> _addUserDtoValidator;
    private readonly IValidator<UpdateUserDto> _updateUserDtoValidator;
    private readonly IValidator<GetUserDto> _getUsersDtoValidator;
    private readonly IValidator<DeleteUserDto> _deleteUserDtoValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IUserRepository userRepository, 
        IValidator<AddUserDto> addUserDtoValidator,
        IValidator<UpdateUserDto> updateUserDtoValidator, 
        IMapper mapper, 
        ILogger<UserService> logger, 
        IValidator<GetUserDto> getUsersDtoValidator, 
        IValidator<DeleteUserDto> deleteUserDtoValidator, 
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher, 
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _addUserDtoValidator = addUserDtoValidator;
        _updateUserDtoValidator = updateUserDtoValidator;
        _getUsersDtoValidator = getUsersDtoValidator;
        _deleteUserDtoValidator = deleteUserDtoValidator;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }


    public async Task<Result<UserDetailsDto, Failure>> AddUserAsync(AddUserDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _addUserDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var password = _passwordHasher.Generate(request.Password);

        var role = await _roleRepository.GetRoleByIdAsync((int)RoleEnum.User);

        if (role == null)
        {
            return Failure.FromError(Error.Validation("RoleNotFound", "Role not found"));
        }
        
        
        var user = UserEntity.Create(
           
                Username: request.Username, 
                Password: password,
                Email: request.Email,
                role
            );
        
        if (await _userRepository.UserExists(user.Username, user.Email)) return Failure.FromError(Error.Validation("UserAlreadyExists", "User already exists", "Username"));
      
        var result = await _userRepository.AddUserAsync(user);
        
        _logger.LogInformation("User created");
        
        var detailsDto = _mapper.Map<UserDetailsDto>(result);
        
        return Result.Success<UserDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(GetUserDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getUsersDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var result = await _userRepository.GetUserById(request.UserId);

        if (result == null) return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));

        var detailsDto = _mapper.Map<UserDetailsDto>(result);

        return Result.Success <UserDetailsDto, Failure>(detailsDto);
    }

    
    
    public async Task<Result<UserDetailsDto, Failure>> UpdateUserAsync(UpdateUserDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateUserDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        var user = await _userRepository.GetByEmail(request.Email);
        
        
        var result = await _userRepository.UpdateUserAsync(user);

        if (result == null) return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));
        var detailsDto = _mapper.Map<UserDetailsDto>(result);
        
        return Result.Success<UserDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<bool, Failure>> DeleteUserAsync(DeleteUserDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _deleteUserDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var result = await _userRepository.DeleteUserAsync(request.Id);
        
        if (!result) return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));
        _logger.LogInformation("User deleted");
        

        return Result.Success<bool, Failure>(result); 
    }

    public async Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserId(GetUserDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getUsersDtoValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var streams = await _userRepository.GetStreamsByUserId(request.UserId);
        
        if (streams == null) return Failure.FromError(Error.Validation("StreamsNotFound", "Streams not found", "UserId"));
        
        var detailsDto = _mapper.Map<List<StreamDetailsDto>>(streams.Streams);
        
        return Result.Success<List<StreamDetailsDto>, Failure>(detailsDto);
    }
    

    public async Task<Result<UserDetailsDto, Failure>> Register(string userName, string email, string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        var roleEntity = await _roleRepository.GetRoleByIdAsync((int)RoleEnum.User);

        if (roleEntity == null)
        {
            return Failure.FromError(Error.Validation("RoleNotFound", "Role not found"));
        }
        
        var user = UserEntity.Create(
            userName, 
            hashedPassword, 
            email,
            roleEntity);

        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }
           
        var exists = await _userRepository.UserExists(userName, email);
        
        UserEntity result;
        
        if (exists)
        {
             return Failure.FromError(Error.Conflict("UserAlreadyExists", "User already exists"));
        }
        else
        {
            result = await _userRepository.AddUserAsync(user);
        }
        
        var detailsDto = _mapper.Map<UserDetailsDto>(result);
        
        return Result.Success<UserDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<string, Failure>> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);

        if (user == null)
        {
            return Failure.FromError(Error.Unauthorized("UserNotFound", "User not found"));
        }
        
        var result = _passwordHasher.Verify(password, user.Password);
        
        if (!result) return Failure.FromError(Error.Unauthorized("InvalidCredentials", "Invalid credentials"));
        
        var token = _jwtProvider.GenerateToken(user);

        return Result.Success<string, Failure>(token);
    }
}