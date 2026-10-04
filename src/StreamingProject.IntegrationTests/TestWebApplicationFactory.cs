using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StreamingProject.Application.Service.Stream.StreamService;

namespace StreamingProject.IntegrationTests;

public sealed class TestWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthenticationHandler.AuthenticationSchemeName;

                    options.DefaultChallengeScheme =
                        TestAuthenticationHandler.AuthenticationSchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationSchemeName,
                    _ => { });

            services.AddSingleton<IAuthorizationHandler,
                TestPermissionAuthorizationHandler>();

            services.RemoveAll<IStreamService>();
            services.AddSingleton<IStreamService, TestStreamService>();
        });
    }
}
