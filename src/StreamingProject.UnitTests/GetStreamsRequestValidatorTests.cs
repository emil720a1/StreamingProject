using FluentAssertions;
using StreamingProject.Application.Service.Stream.StreamValidators;
using StreamingProject.Contracts.Streams;

namespace StreamingProject.UnitTests;

[TestFixture]
public sealed class GetStreamsRequestValidatorTests
{
    private GetStreamsRequestValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new GetStreamsRequestValidator();
    }

    [Test]
    public void Validate_ShouldAcceptDefaultRequest()
    {
        var result = _validator.Validate(new GetStreamsRequestDto());

        result.IsValid.Should().BeTrue();
    }

    [TestCase(0, 12)]
    [TestCase(1, 0)]
    [TestCase(1, 101)]
    public void Validate_ShouldRejectInvalidPagination(int page, int pageSize)
    {
        var result = _validator.Validate(new GetStreamsRequestDto
        {
            Page = page,
            PageSize = pageSize,
        });

        result.IsValid.Should().BeFalse();
    }

    [TestCase("startTime", "asc")]
    [TestCase("startTime", "desc")]
    [TestCase("viewerCount", "asc")]
    [TestCase("VIEWERCOUNT", "DESC")]
    public void Validate_ShouldAcceptSupportedSorting(
        string sortBy,
        string sortDirection)
    {
        var result = _validator.Validate(new GetStreamsRequestDto
        {
            SortBy = sortBy,
            SortDirection = sortDirection,
        });

        result.IsValid.Should().BeTrue();
    }

    [TestCase("title", "desc")]
    [TestCase("startTime", "newest")]
    public void Validate_ShouldRejectUnsupportedSorting(
        string sortBy,
        string sortDirection)
    {
        var result = _validator.Validate(new GetStreamsRequestDto
        {
            SortBy = sortBy,
            SortDirection = sortDirection,
        });

        result.IsValid.Should().BeFalse();
    }
}
