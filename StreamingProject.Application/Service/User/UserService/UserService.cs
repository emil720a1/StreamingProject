using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Application.Service.User.UserService;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IStreamRepository _streamRepository;
    private readonly IValidator<AddUserDto> _addUserDtoValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IUserRepository userRepository, 
        IValidator<AddUserDto> addUserDtoValidator,
        IMapper mapper, 
        ILogger<UserService> logger, 
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher, 
        IRoleRepository roleRepository,
        IStreamRepository streamRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _streamRepository = streamRepository;
        _addUserDtoValidator = addUserDtoValidator;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserDetailsDto, Failure>> RegisterAsync(AddUserDto request, CancellationToken cancellationToken)
    {
        var validation = await _addUserDtoValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return validation.ToErrors();
        
        if (await _userRepository.UserExists(request.Username, request.Email))
            return Failure.FromError(Error.Conflict("User.AlreadyExists", "User already exists"));
        
        var hashedPassword = _passwordHasher.Generate(request.Password);
        var role = await _roleRepository.GetRoleByIdAsync((int)RoleEnum.User);

        if (role == null)
            return Failure.FromError(Error.Validation("RoleNotFound", "Role not found"));
        
        var user = UserEntity.Create(
            request.Username, 
            hashedPassword, 
            request.Email, 
            role,
            request.FirstName ?? string.Empty,
            request.LastName ?? string.Empty);
        
        var result = await _userRepository.AddUserAsync(user);
        _logger.LogInformation("User {Username} registered with ID {UserID}", user.Username, user.Id);
        
        return _mapper.Map<UserDetailsDto>(result);
    }

    public async Task<Result<string, Failure>> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);

        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return Failure.FromError(Error.Unauthorized("UserNotFound", "User not found"));
        }

        return _jwtProvider.GenerateToken(user);
    }

    public async Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserIdAsync(GetUserDto request, CancellationToken cancellationToken)
    {
        var streams = await _streamRepository.GetStreamsByUserId(request.UserId);

        if (streams == null || !streams.Any())
        {
            return Result.Failure<List<StreamDetailsDto>, Failure>(
                Failure.FromError(Error.NotFound("Streams.NotFound", "No streams found for this user", null)));
        }
        
        var streamDto = _mapper.Map<List<StreamDetailsDto>>(streams);
        
        return Result.Success<List<StreamDetailsDto>, Failure>(streamDto);
    }

    public async Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(GetUserDto request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);

        if (user == null)
        {
            return Failure.FromError(Error.NotFound("UserNotFound", "User not found", null));
        }
        
        var userDto = _mapper.Map<UserDetailsDto>(user);
        
        return Result.Success<UserDetailsDto, Failure>(userDto);
    }
}
