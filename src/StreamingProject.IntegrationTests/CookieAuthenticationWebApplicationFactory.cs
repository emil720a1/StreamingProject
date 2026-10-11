using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shared;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Contracts.Streams;
using StreamingProject.Contracts.User;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Presenters.Authentication;
using StreamingProject.Repository.Authentication;

namespace StreamingProject.IntegrationTests;

public sealed class CookieAuthenticationWebApplicationFactory(
    string cookieEnvironment)
    : WebApplicationFactory<Program>
{
    public const string JwtSecret =
        "integration-test-secret-key-with-at-least-32-bytes";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtOptions:SecretKey"] = JwtSecret,
                ["JwtOptions:ExpiresHours"] = "1",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<AuthenticationCookieOptionsFactory>();
            services.AddSingleton(
                new AuthenticationCookieOptionsFactory(
                    new TestHostEnvironment(cookieEnvironment)));

            services.RemoveAll<IUserService>();
            services.AddSingleton<IUserService>(new TestUserService(JwtSecret));

            services.RemoveAll<IStreamService>();
            services.AddSingleton<IStreamService, TestStreamService>();

            var permissionHandlerDescriptor = services.SingleOrDefault(
                descriptor =>
                    descriptor.ServiceType == typeof(IAuthorizationHandler)
                    && descriptor.ImplementationType ==
                    typeof(PermissionAuthorizationHandler));

            if (permissionHandlerDescriptor is not null)
            {
                services.Remove(permissionHandlerDescriptor);
            }

            services.AddSingleton<IAuthorizationHandler,
                TestPermissionAuthorizationHandler>();
        });
    }

    private sealed class TestHostEnvironment(string environmentName)
        : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;

        public string ApplicationName { get; set; } = "StreamingProject.IntegrationTests";

        public string ContentRootPath { get; set; } = string.Empty;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }

    private sealed class TestUserService(string jwtSecret) : IUserService
    {
        public Task<Result<TokenResponse, Failure>> LoginAsync(
            string email,
            string password)
        {
            var response = new TokenResponse(
                CreateAccessToken(jwtSecret),
                "integration-refresh-token");

            return Task.FromResult(
                Result.Success<TokenResponse, Failure>(response));
        }

        public Task<Result<TokenResponse, Failure>> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            var response = new TokenResponse(
                CreateAccessToken(jwtSecret),
                "rotated-integration-refresh-token");

            return Task.FromResult(
                Result.Success<TokenResponse, Failure>(response));
        }

        public Task<Result<UserDetailsDto, Failure>> RegisterAsync(
            AddUserDto request,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Result<List<StreamListItemDto>, Failure>> GetStreamsByUserIdAsync(
            GetUserDto request,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<Result<UserDetailsDto, Failure>> GetUserByIdAsync(
            GetUserDto request,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        private static string CreateAccessToken(string secret)
        {
            Claim[] claims =
            [
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim("userName", "integration-user"),
                new Claim("email", "integration@example.com"),
                new Claim("test_permission", "Read"),
            ];

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
