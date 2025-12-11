
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using StreamingProject.Application;
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




app.Run();