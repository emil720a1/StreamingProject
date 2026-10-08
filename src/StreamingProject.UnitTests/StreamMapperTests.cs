using AutoMapper;
using FluentAssertions;
using Shared.Common;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain.Stream;

namespace StreamingProject.UnitTests;

[TestFixture]
public class StreamMapperTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var configuration = new MapperConfiguration(
            configuration =>
            {
                configuration.AddProfile<StreamMapper>();
            });

        _mapper = configuration.CreateMapper();
    }

    [Test]
    public void ShouldMapStreamEntityToStreamListItemDto()
    {
        var userId = Guid.NewGuid();
        var stream = StreamEntity.Create(
            userId,
            "Test stream",
            "Test description",
            "Gaming",
            null);

        var result = _mapper.Map<StreamListItemDto>(stream);

        result.Should().NotBeNull();
        result.Id.Should().Be(stream.Id);
        result.UserId.Should().Be(stream.UserId);
        result.Title.Should().Be(stream.Title);
        result.Description.Should().Be(stream.Description);
        stream.Title.Should().Be("Test stream");
        stream.Description.Should().Be("Test description");
        stream.Category.Should().Be("Gaming");
        stream.ThumbnailUrl.Should().BeNull();
        result.StartTime.Should().Be(stream.StartTime);
    }

    [Test]
    public void ShouldMapStreamEntityToCreateStreamResponseDtoWithStreamKey()
    {
        var stream = StreamEntity.Create(
            Guid.NewGuid(),
            "Test stream",
            "Test description",
            "Gaming",
            null);

        var result = _mapper.Map<CreateStreamResponseDto>(stream);

        result.Id.Should().Be(stream.Id);
        result.StreamKey.Should().Be(stream.StreamKey);
    }
}
