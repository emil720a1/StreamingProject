using AutoMapper;
using FluentAssertions;
using FluentValidation;
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
        _hlsServiceMock = new Mock<IHlsTranscoderService>();

        _sut = new StreamService(
            _repositoryMock.Object,
            _loggerMock.Object,
            _createValidatorMock.Object,
            _mapperMock.Object,
            _joinValidatorMock.Object,
            _hlsServiceMock.Object);
    }


    [Test]
    public async Task GetAvailableStreamAsync_ShouldReturnStreams()
    {
        var userId = Guid.NewGuid();

        var entities = new List<StreamEntity>
        {
            StreamEntity.Create(userId, "Gaming Live", "Gaming stream", "Gaming", null),
            StreamEntity.Create(userId, "Music Session", "Music stream", "Music", null),
        };

        var expectedDtos = new List<StreamListItemDto>
        {
            new(
                entities[0].Id,
                entities[0].UserId,
                "alex",
                "Gaming Live",
                "Gaming stream",
                DateTime.UtcNow),
            new(
                entities[1].Id,
                entities[1].UserId,
                "maria",
                "Music Session",
                "Music stream",
                DateTime.UtcNow),
        };

        _repositoryMock
            .Setup(repository => repository.GetAvailableStreamsAsync())
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(mapper => mapper.Map<List<StreamListItemDto>>(entities))
            .Returns(expectedDtos);

        var result = await _sut.GetAvailableStreamsAsync(
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDtos);

        _repositoryMock.Verify(
            repository => repository.GetAvailableStreamsAsync(),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<List<StreamListItemDto>>(entities),
            Times.Once);
    }

    [Test]
    public async Task GetAvailableStreamsAsync_ShouldReturnEmptyList_WhenNoStreamsExist()
    {
        var entities = new List<StreamEntity>();
        var expectedDtos = new List<StreamListItemDto>();

        _repositoryMock
            .Setup(repository => repository.GetAvailableStreamsAsync())
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(mapper => mapper.Map<List<StreamListItemDto>>(entities))
            .Returns(expectedDtos);

        var result = await _sut.GetAvailableStreamsAsync(
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _repositoryMock.Verify(
            repository => repository.GetAvailableStreamsAsync(),
            Times.Once);
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

}
