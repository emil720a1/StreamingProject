using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using StreamingProject.Application.Interfaces.Hls;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain.Stream;

namespace StreamingProject.UnitTests;

[TestFixture]
public sealed class StreamServiceTests
{
    private Mock<IStreamRepository> _repositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<ILogger<StreamService>> _loggerMock = null!;
    private Mock<IValidator<CreateStreamDto>> _createValidatorMock = null!;
    private Mock<IValidator<JoinStreamDto>> _joinValidatorMock = null!;
    private Mock<IValidator<GetStreamsRequestDto>> _getStreamsValidatorMock = null!;
    private Mock<IHlsTranscoderService> _hlsServiceMock = null!;

    private StreamService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IStreamRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<StreamService>>();
        _createValidatorMock = new Mock<IValidator<CreateStreamDto>>();
        _joinValidatorMock = new Mock<IValidator<JoinStreamDto>>();
        _getStreamsValidatorMock = new Mock<IValidator<GetStreamsRequestDto>>();
        _hlsServiceMock = new Mock<IHlsTranscoderService>();

        _getStreamsValidatorMock
            .Setup(validator => validator.ValidateAsync(
                It.IsAny<GetStreamsRequestDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _sut = new StreamService(
            _repositoryMock.Object,
            _loggerMock.Object,
            _createValidatorMock.Object,
            _mapperMock.Object,
            _joinValidatorMock.Object,
            _getStreamsValidatorMock.Object,
            _hlsServiceMock.Object);
    }


    [Test]
    public async Task GetAvailableStreamAsync_ShouldReturnStreams()
    {
        var userId = Guid.NewGuid();

        var request = new GetStreamsRequestDto();
        var items = new List<StreamListItemReadModel>
        {
            new(
                Guid.NewGuid(),
                userId,
                "alex",
                "Gaming Live",
                "Gaming stream",
                DateTime.UtcNow,
                "Gaming",
                "gaming.jpg",
                15),
            new(
                Guid.NewGuid(),
                userId,
                "maria",
                "Music Session",
                "Music stream",
                DateTime.UtcNow,
                "Music",
                null,
                8),
        };

        _repositoryMock
            .Setup(repository => repository.GetAvailableStreamsAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StreamPage(items, 14));

        var result = await _sut.GetAvailableStreamsAsync(
            request,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(14);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(12);
        result.Value.TotalPages.Should().Be(2);
        result.Value.Items[0].ViewerCount.Should().Be(15);

        _repositoryMock.Verify(
            repository => repository.GetAvailableStreamsAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetAvailableStreamsAsync_ShouldReturnEmptyList_WhenNoStreamsExist()
    {
        var request = new GetStreamsRequestDto();

        _repositoryMock
            .Setup(repository => repository.GetAvailableStreamsAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StreamPage([], 0));

        var result = await _sut.GetAvailableStreamsAsync(
            request,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalPages.Should().Be(0);

        _repositoryMock.Verify(
            repository => repository.GetAvailableStreamsAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetAvailableStreamsAsync_ShouldNotQueryRepository_WhenRequestIsInvalid()
    {
        var request = new GetStreamsRequestDto { Page = 0 };
        _getStreamsValidatorMock
            .Setup(validator => validator.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
            [
                new ValidationFailure(nameof(request.Page), "Page must be at least 1."),
            ]));

        var result = await _sut.GetAvailableStreamsAsync(
            request,
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(
            repository => repository.GetAvailableStreamsAsync(
                It.IsAny<GetStreamsRequestDto>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetStreamStatusAsync_ShouldReturnPreparing_WhenStreamHasNotStarted()
    {
        var userId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            userId,
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);

        _repositoryMock
            .Setup(repository => repository.GetStreamByIdAsync(stream.Id))
            .ReturnsAsync(stream);

        var result = await _sut.GetStreamStatusAsync(
            stream.Id,
            userId,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new StreamStatusDto(stream.Id, "Preparing"));
    }

    [Test]
    public async Task GetStreamStatusAsync_ShouldReturnLive_WhenStreamHasStarted()
    {
        var userId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            userId,
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);
        stream.StartStream();

        _repositoryMock
            .Setup(repository => repository.GetStreamByIdAsync(stream.Id))
            .ReturnsAsync(stream);

        var result = await _sut.GetStreamStatusAsync(
            stream.Id,
            userId,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Live");
    }

    [Test]
    public async Task GetStreamStatusAsync_ShouldReturnEnded_WhenStreamHasEnded()
    {
        var userId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            userId,
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);
        stream.StartStream();
        stream.EndStream();

        _repositoryMock
            .Setup(repository => repository.GetStreamByIdAsync(stream.Id))
            .ReturnsAsync(stream);

        var result = await _sut.GetStreamStatusAsync(
            stream.Id,
            userId,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Ended");
    }

    [Test]
    public async Task GetStreamStatusAsync_ShouldFail_WhenUserDoesNotOwnStream()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            ownerId,
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);

        _repositoryMock
            .Setup(repository => repository.GetStreamByIdAsync(stream.Id))
            .ReturnsAsync(stream);

        var result = await _sut.GetStreamStatusAsync(
            stream.Id,
            otherUserId,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task StartStreamByKeyAsync_ShouldStartAndPersistStream()
    {
        var stream = StreamEntity.Create(
            Guid.NewGuid(),
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);

        _repositoryMock
            .Setup(repository => repository.GetStreamByKeyAsync(stream.StreamKey))
            .ReturnsAsync(stream);
        _repositoryMock
            .Setup(repository => repository.UpdateStreamAsync(stream))
            .ReturnsAsync(stream);
        _hlsServiceMock
            .Setup(service => service.StartTranscodingAsync(
                stream.StreamKey,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.StartStreamByKeyAsync(
            stream.StreamKey,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        stream.StartTime.Should().NotBeNull();
        stream.EndTime.Should().BeNull();
        _repositoryMock.Verify(
            repository => repository.UpdateStreamAsync(stream),
            Times.Once);
        _hlsServiceMock.Verify(
            service => service.StartTranscodingAsync(
                stream.StreamKey,
                It.Is<string>(path => path.EndsWith(
                    Path.Combine("wwwroot", "hls", stream.Id.ToString()))),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task EndStreamByKeyAsync_ShouldEndPersistAndStopTranscoding()
    {
        var stream = StreamEntity.Create(
            Guid.NewGuid(),
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);
        stream.StartStream();

        _repositoryMock
            .Setup(repository => repository.GetStreamByKeyAsync(stream.StreamKey))
            .ReturnsAsync(stream);
        _repositoryMock
            .Setup(repository => repository.UpdateStreamAsync(stream))
            .ReturnsAsync(stream);
        _hlsServiceMock
            .Setup(service => service.StopTranscodingAsync(
                stream.StreamKey,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.EndStreamByKeyAsync(
            stream.StreamKey,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        stream.EndTime.Should().NotBeNull();
        _repositoryMock.Verify(
            repository => repository.UpdateStreamAsync(stream),
            Times.Once);
        _hlsServiceMock.Verify(
            service => service.StopTranscodingAsync(
                stream.StreamKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task EndStreamByKeyAsync_ShouldReturnFailureAndNotPersist_WhenTranscodingCannotStop()
    {
        var stream = StreamEntity.Create(
            Guid.NewGuid(),
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);
        stream.StartStream();

        _repositoryMock
            .Setup(repository => repository.GetStreamByKeyAsync(stream.StreamKey))
            .ReturnsAsync(stream);
        _hlsServiceMock
            .Setup(service => service.StopTranscodingAsync(
                stream.StreamKey,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Failure.FromError(
                Error.Failure("Hls.StopFailed", "Unable to stop transcoding")));

        var result = await _sut.EndStreamByKeyAsync(
            stream.StreamKey,
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        stream.EndTime.Should().BeNull();
        _repositoryMock.Verify(
            repository => repository.UpdateStreamAsync(It.IsAny<StreamEntity>()),
            Times.Never);
    }

    [Test]
    public async Task EndStreamAsync_ShouldReturnFailureAndNotPersist_WhenTranscodingCannotStop()
    {
        var userId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            userId,
            "Gaming Live",
            "Gaming stream",
            "Gaming",
            null);
        stream.StartStream();

        _repositoryMock
            .Setup(repository => repository.GetStreamByIdAsync(stream.Id))
            .ReturnsAsync(stream);
        _hlsServiceMock
            .Setup(service => service.StopTranscodingAsync(
                stream.StreamKey,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Failure.FromError(
                Error.Failure("Hls.StopFailed", "Unable to stop transcoding")));

        var result = await _sut.EndStreamAsync(
            new EndStreamDto(stream.Id, userId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        stream.EndTime.Should().BeNull();
        _repositoryMock.Verify(
            repository => repository.UpdateStreamAsync(It.IsAny<StreamEntity>()),
            Times.Never);
    }

}
