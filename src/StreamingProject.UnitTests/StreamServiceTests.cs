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
            StreamEntity.Create(userId),
            StreamEntity.Create(userId),
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

}
