
using System.Net;
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
using StreamProject.Web;
using StreamProject.Web.Extensions;
using StreamProject.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


services.AddApiAuthentication(configuration);

    services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
    services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));

    services.AddDbContext<StreamingDbContext>(options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(StreamingDbContext)));
    });

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
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetService<StreamingDbContext>();
//     await dbContext.Database.MigrateAsync();
// }


await app.RunAsync();

