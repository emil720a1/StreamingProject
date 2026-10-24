using System.Net;
using Extensions.Hosting.AsyncInitialization;
using LiveStreamingServerNet;
using LiveStreamingServerNet.Networking;
using LiveStreamingServerNet.Rtmp.Server.Contracts;
using LiveStreamingServerNet.Rtmp.Server.Installer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using Shared.Common;
using StreamingProject.Application.Observability;
using StreamingProject.Presenters.Authentication;
using StreamingProject.Presenters.Handlers;
using StreamingProject.Repository;
using StreamingProject.Repository.Authentication;
using StreamingProject.Repository.Repositories.UserRepositories;
using StreamingProject.Repository.Repositories.RoleRepositories;
using StreamProject.Web;
using StreamProject.Web.Extensions;
using StreamProject.Web.Middlewares;
using Shared.Correlation;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.SingleLine = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss  ";
        options.UseUtcTimestamp = true;
    });
}
else
{
    builder.Logging.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        options.UseUtcTimestamp = true;
    });
}

// builder.Logging.AddJsonConsole(options =>
// {
//     options.IncludeScopes = true;
//     options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
//     options.UseUtcTimestamp = true;
// });

var services = builder.Services;
var configuration = builder.Configuration;

services.AddScoped<ICorrelationContext, CorrelationContext>();

services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

services.AddSingleton<StreamingMetrics>();

services.AddHealthChecks()
    .AddDbContextCheck<StreamingDbContext>("postgresql");

services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService("streaming-backend");
    })
    .WithMetrics(metrics =>
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter()
            .AddMeter(StreamingMetrics.MeterName))
    .WithTracing(tracing =>
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(StreamingActivitySource.Name)
            .AddOtlpExporter(options =>
            {
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Endpoint = new Uri(
                    configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                    ?? "http://localhost:4317/v1/traces");
            }));

services.AddSingleton<AuthenticationCookieOptionsFactory>();
services.AddApiAuthentication(configuration);

    services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
    services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));

    var connectionString = configuration.GetConnectionString(nameof(StreamingDbContext));
    if (!builder.Environment.IsEnvironment("Testing")
        && string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "ConnectionStrings:StreamingDbContext is missing. " +
            "Run the application through Docker Compose or configure the connection string in appsettings.Development.json.");
    }

    services.AddDbContext<StreamingDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });

services.AddScoped<ISeeder, RoleSeeder>();
services.AddScoped<ISeeder, UserSeeder>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSingleton<IRtmpServerStreamEventHandler, RtmpServerEventHandler>();

    var serverEndPoint = new ServerEndPoint(
        new IPEndPoint(IPAddress.Any, 1935),
        false);

    services.AddLiveStreamingServer(serverEndPoint, rtmp =>
    {
        rtmp.AddStreamEventHandler<RtmpServerEventHandler>();
    });
}

services.AddAutoMapper(typeof(StreamMapper));
services.AddProgramDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StreamingProject.Web"));
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Unspecified,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest,
});

// if (!app.Environment.IsDevelopment()
//     && !app.Environment.IsEnvironment("Testing"))
// {
//     app.UseHttpsRedirection();
// }

app.MapHealthChecks("/health");

app.MapPrometheusScrapingEndpoint();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var databaseInitializationEnabled =
    configuration.GetValue<bool?>("DatabaseInitialization:Enabled")
    ?? app.Environment.IsDevelopment();

if (databaseInitializationEnabled
    && !app.Environment.IsEnvironment("Testing"))
{
    await app.InitAsync();
}

app.UseStaticFiles();

app.MapHub<StreamingProject.Presenters.Hubs.ChatHub>("/chatHub");

await app.RunAsync();

public partial class Program
{
}
