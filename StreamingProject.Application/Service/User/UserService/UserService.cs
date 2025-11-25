using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Domain.User;
using StreamingProject.Infrastructure.PasswordHasher;

namespace StreamingProject.Application.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<AddUserDto> _addUserDtoValidator;
    private readonly IValidator<UpdateUserDto> _updateUserDtoValidator;
    private readonly IValidator<GetUserDto> _getUsersDtoValidator;
    private readonly IValidator<DeleteUserDto> _deleteUserDtoValidator;
    private readonly IValidator<GetStreamsByUserId> _getStreamsByUserId;
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
        IValidator<GetStreamsByUserId> getStreamsByUserId, 
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _addUserDtoValidator = addUserDtoValidator;
        _updateUserDtoValidator = updateUserDtoValidator;
        _getUsersDtoValidator = getUsersDtoValidator;
        _deleteUserDtoValidator = deleteUserDtoValidator;
        _getStreamsByUserId = getStreamsByUserId;
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

        var user = UserEntity.Create(
           
                Username: request.Username, 
                Password: request.Password,
                Email: request.Email
            );
        

        if (await _userRepository.UserExists(user.Username)) return Failure.FromError(Error.Validation("UserAlreadyExists", "User already exists", "Username"));
      
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
        var user = UserEntity.Create(
           
            Username: request.Username, 
            Password: request.Password,
            Email: request.Email
        );
        
        
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

    public async Task<Result<List<StreamDetailsDto>, Failure>> GetStreamsByUserId(GetStreamsByUserId request, CancellationToken cancellationToken)
    {
        var validationResult = await _getStreamsByUserId.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }
        
        var streams = await _userRepository.GetStreamsByUserId(request.UserId);
        
        if (streams == null) return Failure.FromError(Error.Validation("StreamsNotFound", "Streams not found", "UserId"));
        
        var detailsDto = _mapper.Map<List<StreamDetailsDto>>(streams);
        
        return Result.Success<List<StreamDetailsDto>, Failure>(detailsDto);
    }
    

    public async Task<Result<UserDetailsDto, Failure>> Register(string userName, string email, string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        var user = UserEntity.Create(userName, hashedPassword, email);

        var result = await _userRepository.AddUserAsync(user);

        if (result == null)
        {
            return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));
        }
        
        var detailsDto = _mapper.Map<UserDetailsDto>(result);
        
        return Result.Success<UserDetailsDto, Failure>(detailsDto);
    }

    public async Task<Result<string, Failure>> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);

        if (user == null)
        {
            return Failure.FromError(Error.Validation("UserNotFound", "User not found", "UserId"));
        }
        var result = _passwordHasher.Verify(password, user.Password);

        if (result == false)
        {
           return Failure.FromError(Error.Validation("UserNotFound",  "User not found", "UserId"));
        }
        
        var token = _jwtProvider.GenerateToken(user);

        return Result.Success<string, Failure>(token);
    }
}