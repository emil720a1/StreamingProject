using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using StreamingProject.Application.Service.Subscription.SubscriptionRepository;
using StreamingProject.Application.Service.Subscription.SubscriptionService;
using StreamingProject.Contracts.SubscriptionsContracts;
using StreamingProject.Domain.Subscription;

namespace StreamingProject.UnitTests;

[TestFixture]
public class SubscriptionServiceTests
{
    private Mock<ISubscriptionRepository> _repositoryMock;
    private Mock<IValidator<SubscriptionDto>> _validatorMock; 
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<SubscriptionService>> _loggerMock;
    private SubscriptionService _sut;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ISubscriptionRepository>();
        _validatorMock = new Mock<IValidator<SubscriptionDto>>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<SubscriptionService>>();

        _sut = new SubscriptionService(
            _repositoryMock.Object,
            _validatorMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task SubscribeAsync_ValidSubscription_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var request = new SubscriptionDto(
            Guid.NewGuid(),
            userId,
            userId,
            DateTime.UtcNow);
        
        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        var result = await _sut.SubscribeAsync(request, CancellationToken.None);
        
        result.IsFailure.Should().BeTrue();
        result.Error.First().Code.Should().Be("Subscription.Self");
        
        _repositoryMock.Verify(
            r => r.SubscribeAsync(
                It.IsAny<SubscriptionEntity>(), 
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Test]
    public async Task SubscribeAsync_ShouldReturnSuccess_WhenSubscriptionIsValid()
    {
        var followerId = Guid.NewGuid();
        var followedId =  Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        
        var request = new SubscriptionDto(Guid.NewGuid(), followerId, followedId, DateTime.UtcNow);
        var subscriptionEntity = SubscriptionEntity.Create(followerId, followedId);
        
        var expectedDetails = new SubscriptionDetailsDto(subscriptionId, followedId, followerId, DateTime.UtcNow);
        
        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        _repositoryMock.Setup(r => r.SubscribeAsync(It.IsAny<SubscriptionEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptionEntity);
        
        _mapperMock.Setup(m => m.Map<SubscriptionDetailsDto>(It.IsAny<SubscriptionEntity>()))
            .Returns(expectedDetails);
        
        var result = await _sut.SubscribeAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(subscriptionId);
        result.Value.FollowerId.Should().Be(followerId);
        
        _repositoryMock.Verify(r => r.SubscribeAsync(It.IsAny<SubscriptionEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UnsubscribeAsync_ShouldReturnNotFound_WhenSubscriptionDoesNotExist()
    {
        var request = new UnSubscribeDto(Guid.NewGuid(), Guid.NewGuid());

        _repositoryMock.Setup(r => r.UnsubscribeAsync(request.FollowerId, request.FollowedId))
            .ReturnsAsync(false);

        var result = await _sut.UnsubscribeAsync(request, CancellationToken.None);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Never);
    }
}