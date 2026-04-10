using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Domain.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingProject.UnitTests;

[TestFixture]
public class UserServiceTests
{
    private Mock<UserManager<UserEntity>> _userManagerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IValidator<AddUserDto>> _validatorMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private Mock<IJwtProvider> _jwtProviderMock;
    private Mock<IStreamRepository> _streamRepositoryMock;
    
    private UserService _sut;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<UserEntity>>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<AddUserDto>>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _streamRepositoryMock = new Mock<IStreamRepository>();
        
        _sut = new UserService(
            _userManagerMock.Object,
            _validatorMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _jwtProviderMock.Object,
            _streamRepositoryMock.Object);
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        var request = new AddUserDto("", "", "", "", "");
        var validationResult = new FluentValidation.Results.ValidationResult(new[] 
        { 
            new FluentValidation.Results.ValidationFailure("Username", "Required") 
        });

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _sut.RegisterAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenIdentitySucceeds()
    {
        var request = new AddUserDto("new_user", "Pass123!", "new@test.com", "Oleg", "Dev");
        
        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var expectedDto = new UserDetailsDto(Guid.NewGuid(), "new_user", "Oleg", "Dev", new List<string> { "User" }, UserStatus.Active);
        _mapperMock.Setup(m => m.Map<UserDetailsDto>(It.IsAny<UserEntity>()))
            .Returns(expectedDto);

        var result = await _sut.RegisterAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("new_user");
    }

    [Test]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var email = "test@test.com";
        var password = "password";
        var user = new UserEntity { Email = email, UserName = "testuser" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);

        _jwtProviderMock.Setup(j => j.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenResponse("fake-token", "fake-refresh"));

        var result = await _sut.LoginAsync(email, password);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("fake-token");
        result.Value.RefreshToken.Should().Be("fake-refresh");
    }
}