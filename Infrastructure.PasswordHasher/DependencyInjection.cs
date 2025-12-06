using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Infrastructure.PasswordHasher.Authentication;

namespace StreamingProject.Infrastructure.PasswordHasher;

public static class DependencyInjection
{
    public static IServiceCollection AddPasswordInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, Authentication.PasswordHasher>();
        
        services.AddScoped<IJwtProvider, JwtProvider>();
        
        
        
        return services;
    }
} 