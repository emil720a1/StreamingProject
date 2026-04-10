using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;
using StreamingProject.Repository;
using StreamingProject.Repository.Authentication;

namespace StreamProject.Web.Extensions;

public static class ApiExtensions
{
    public static void AddApiAuthentication(
        this IServiceCollection services,
       IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        services.AddIdentity<UserEntity, RoleEntity>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<StreamingDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
            };
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["tasty-cookies"];
                    return Task.CompletedTask;
                }
            };
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        
        services.AddAuthorization(options =>
        {
            var permissions = Enum.GetValues<PermissionEnum>();

            foreach (var permission in permissions)
            {
                var policyName = $"Permission.{permission}";
                options.AddPolicy(policyName, policy =>
                {
                    policy.AddRequirements(new PermissionRequirement([permission]));
                    policy.RequireAuthenticatedUser();
                });
            }
        });
    }
}