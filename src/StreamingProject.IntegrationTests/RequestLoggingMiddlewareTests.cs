using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StreamProject.Web.Middlewares;

namespace StreamingProject.IntegrationTests;

[TestFixture]
public sealed class RequestLoggingMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_ShouldLogRequestMethodPathStatusAndDuration()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/Streams";
        var logger = new TestLogger<RequestLoggingMiddleware>();

        var middleware = CreateMiddleware(logger, httpContext =>
        {
            httpContext.Response.StatusCode = StatusCodes.Status201Created;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        logger.Messages.Should().ContainSingle();
        logger.Messages[0].Should().Contain("POST /api/Streams responded 201");
        logger.Messages[0].Should().Contain(" ms");
    }

    [Test]
    public async Task InvokeAsync_ShouldLogUnauthorizedResponse()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/Streams";
        var logger = new TestLogger<RequestLoggingMiddleware>();

        var middleware = CreateMiddleware(logger, httpContext =>
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        logger.Messages.Should().ContainSingle()
            .Which.Should().Contain("GET /api/Streams responded 401");
    }

    [Test]
    public async Task InvokeAsync_ShouldCallNextMiddlewareExactlyOnce()
    {
        var context = new DefaultHttpContext();
        var logger = new TestLogger<RequestLoggingMiddleware>();
        var nextCalls = 0;

        var middleware = CreateMiddleware(logger, _ =>
        {
            nextCalls++;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        nextCalls.Should().Be(1);
        logger.Messages.Should().ContainSingle();
    }

    private static RequestLoggingMiddleware CreateMiddleware(
        ILogger<RequestLoggingMiddleware> logger,
        RequestDelegate next)
    {
        return new RequestLoggingMiddleware(next, logger);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
