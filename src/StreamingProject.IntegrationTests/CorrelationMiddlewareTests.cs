using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Shared.Correlation;
using StreamProject.Web.Middlewares;

namespace StreamingProject.IntegrationTests;

[TestFixture]
public sealed class CorrelationMiddlewareTests
{
    private const string HeaderName = "X-Correlation-ID";

    [Test]
    public async Task InvokeAsync_ShouldPreserveIncomingCorrelationId()
    {
        var context = new DefaultHttpContext();
        const string correlationId = "request-123";
        context.Request.Headers[HeaderName] = correlationId;

        var nextCalled = false;
        var correlationContext = new CorrelationContext();
        var middleware = CreateMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, correlationContext);

        await middleware.InvokeAsync(context);

        context.Response.Headers[HeaderName].ToString().Should().Be(correlationId);
        correlationContext.CorrelationId.Should().Be(correlationId);
        nextCalled.Should().BeTrue();
    }

    [Test]
    public async Task InvokeAsync_ShouldGenerateCorrelationId_WhenHeaderIsMissing()
    {
        var context = new DefaultHttpContext();
        var correlationContext = new CorrelationContext();
        var middleware = CreateMiddleware(_ => Task.CompletedTask, correlationContext);

        await middleware.InvokeAsync(context);

        var correlationId = context.Response.Headers[HeaderName].ToString();

        correlationId.Should().NotBeNullOrWhiteSpace();
        correlationId.Should().HaveLength(32);
        Guid.TryParseExact(correlationId, "N", out _).Should().BeTrue();
        correlationContext.CorrelationId.Should().Be(correlationId);
    }

    [Test]
    public async Task InvokeAsync_ShouldGenerateNewCorrelationId_WhenHeaderIsTooLong()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderName] = new string('a', 129);
        var correlationContext = new CorrelationContext();
        var middleware = CreateMiddleware(_ => Task.CompletedTask, correlationContext);

        await middleware.InvokeAsync(context);

        var correlationId = context.Response.Headers[HeaderName].ToString();

        correlationId.Should().HaveLength(32);
        Guid.TryParseExact(correlationId, "N", out _).Should().BeTrue();
        correlationContext.CorrelationId.Should().Be(correlationId);
    }

    [Test]
    public async Task InvokeAsync_ShouldCallNextMiddlewareExactlyOnce()
    {
        var context = new DefaultHttpContext();
        var nextCalls = 0;
        var correlationContext = new CorrelationContext();
        var middleware = CreateMiddleware(_ =>
        {
            nextCalls++;
            return Task.CompletedTask;
        }, correlationContext);

        await middleware.InvokeAsync(context);

        nextCalls.Should().Be(1);
    }

    private static CorrelationIdMiddleware CreateMiddleware(
        RequestDelegate next,
        ICorrelationContext correlationContext)
    {
        return new CorrelationIdMiddleware(
            next,
            NullLogger<CorrelationIdMiddleware>.Instance,
            correlationContext);
    }
}
