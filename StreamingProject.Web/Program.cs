using StreamingProject.Presenters.Authentication;
using System.Net;
using Extensions.Hosting.AsyncInitialization;
using LiveStreamingServerNet;
using LiveStreamingServerNet.Networking;
using LiveStreamingServerNet.Rtmp.Server.Contracts;
using LiveStreamingServerNet.Rtmp.Server.Installer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using StreamingProject.Presenters.Handlers;
using StreamingProject.Repository;
using StreamingProject.Repository.Authentication;
using StreamingProject.Repository.Repositories.UserRepositories;
using StreamingProject.Repository.Repositories.RoleRepositories;
using StreamProject.Web;
using StreamProject.Web.Extensions;
using StreamProject.Web.Middlewares;
using StreamProject.Web.Seeders;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

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

services.AddSingleton<AuthenticationCookieOptionsFactory>();
services.AddApiAuthentication(configuration);

    services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
    services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));

    services.AddDbContext<StreamingDbContext>(options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(StreamingDbContext)));
    });

services.AddScoped<ISeeder, RoleSeeder>();
services.AddScoped<ISeeder, UserSeeder>();

    builder.Services.AddSingleton<IRtmpServerStreamEventHandler, RtmpServerEventHandler>();

    var serverEndPoint = new ServerEndPoint(new IPEndPoint(IPAddress.Any, 1935), false);

    services.AddLiveStreamingServer(serverEndPoint, rtmp =>
    {
        rtmp.AddStreamEventHandler<RtmpServerEventHandler>();

     });

services.AddLogging(logging => logging.AddConsole());

services.AddAutoMapper(typeof(StreamMapper));
services.AddProgramDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StreamingProject.Web"));
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Unspecified,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = app.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always
});

if (!app.Environment.IsDevelopment()
    && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetService<StreamingDbContext>();
//     await dbContext.Database.MigrateAsync();
// }

if (!app.Environment.IsEnvironment("Testing"))
{
    await app.InitAsync();
    await app.UseSeeders();
}

app.UseStaticFiles();

app.MapHub<StreamingProject.Presenters.Hubs.ChatHub>("/chatHub");

await app.RunAsync();

public partial class Program
{
}
