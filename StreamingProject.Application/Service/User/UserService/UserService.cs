using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Extensions;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Domain.User;

namespace StreamingProject.Application.Service.User.UserService;

public class UserService : IUserService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IStreamRepository _streamRepository;
    private readonly IValidator<AddUserDto> _addUserDtoValidator;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    
    public UserService(
        UserManager<UserEntity> userManager, 
        IValidator<AddUserDto> addUserDtoValidator,
        IMapper mapper, 
        ILogger<UserService> logger, 
        IJwtProvider jwtProvider,
        IStreamRepository streamRepository)
    {
        _userManager = userManager;
        _addUserDtoValidator = addUserDtoValidator;
        _jwtProvider = jwtProvider;
        _mapper = mapper;
        _logger = logger;
        _streamRepository = streamRepository;
    }

    public async Task<Result<UserDetailsDto, Failure>> RegisterAsync(AddUserDto request, CancellationToken cancellationToken)
    {
        var validation = await _addUserDtoValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return validation.ToErrors();
        
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            Status = UserStatus.Active
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
            return new Failure(errors);
        }

        // Default role assignment
        await _userManager.AddToRoleAsync(user, "User");

        _logger.LogInformation("User {Username} registered with ID {UserID}", user.UserName, user.Id);
        
        return _mapper.Map<UserDetailsDto>(user);
    }

    public async Task<Result<TokenResponse, Failure>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Failure.FromError(Error.Unauthorized("InvalidCredentials", "Invalid email or password"));
        }

        var tokenResponse = await _jwtProvider.GenerateTokenAsync(user);
        return Result.Success<TokenResponse, Failure>(tokenResponse);
    }

    public async Task<Result<TokenResponse, Failure>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenResponse = await _jwtProvider.RefreshTokenAsync(refreshToken, cancellationToken);
            return Result.Success<TokenResponse, Failure>(tokenResponse);
        }
        catch (Exception ex)
        {
            return Failure.FromError(Error.Unauthorized("InvalidToken", ex.Message));
        }
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
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return Failure.FromError(Error.NotFound("UserNotFound", "User not found", null));
        }
        
        var userDto = _mapper.Map<UserDetailsDto>(user);
        
        return Result.Success<UserDetailsDto, Failure>(userDto);
    }
}
