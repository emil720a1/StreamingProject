using Shared.Correlation;

namespace StreamProject.Web.Middlewares;

public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly ICorrelationContext _correlationContext;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger,
        ICorrelationContext correlationContext)
    {
        _next = next;
        _logger = logger;
        _correlationContext = correlationContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName]
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId)
            || correlationId.Length > 128)
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Response.Headers[HeaderName] = correlationId;

        _correlationContext.CorrelationId = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            await _next(context);
        }
    }
}