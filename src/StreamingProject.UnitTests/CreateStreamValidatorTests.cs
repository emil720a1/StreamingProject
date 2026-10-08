using FluentAssertions;
using StreamingProject.Application.Service.Stream.StreamValidators;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.UnitTests;

[TestFixture]
public class CreateStreamValidatorTests
{
    private CreateStreamValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateStreamValidator();
    }

    [Test]
    public async Task Should_pass_for_valid_request()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            "Gaming Live",
            "My gaming stream",
            "Gaming",
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }


    [Test]
    public async Task Should_fail_when_title_is_empty()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            string.Empty,
            "My gaming stream",
            "Gaming",
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Title");
    }

    [Test]
    public async Task Should_fail_when_category_is_empty()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            "Gaming Live",
            "My gaming stream",
            string.Empty,
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Category");
    }

    [Test]
    public async Task Should_fail_when_title_is_too_short()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            "ab",
            "My gaming stream",
            "Gaming",
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Title");
    }

    [Test]
    public async Task Should_fail_when_title_is_too_long()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            new string('a', 101),
            "My gaming stream",
            "Gaming",
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Title");
    }

    [Test]
    public async Task Should_fail_when_description_is_empty()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            "Gaming Live",
            string.Empty,
            "Gaming",
            null);

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "Description");
    }

    [Test]
    public async Task Should_fail_when_thumbnail_url_is_too_long()
    {
        var request = new CreateStreamDto(
            Guid.NewGuid(),
            "Gaming Live",
            "My gaming stream",
            "Gaming",
            new string('a', 2049));

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == "ThumbnailUrl");
    }
}
