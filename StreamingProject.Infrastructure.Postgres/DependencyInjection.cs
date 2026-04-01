using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Application.Service.Chat.ChatRepository;
using StreamingProject.Application.Service.Permission.PermissionRepository;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Application.Service.Stream.StreamRepository;
using StreamingProject.Application.Service.Subscription.SubscriptionRepository;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Application.Service.Video;
using StreamingProject.Application.Service.Video.VideoRepository;
using StreamingProject.Repository.Authentication;
using StreamingProject.Repository.Repositories;
using StreamingProject.Repository.Repositories.ChatRepositories;
using StreamingProject.Repository.Repositories.PermissionRepositories;
using StreamingProject.Repository.Repositories.RoleRepositories;
using StreamingProject.Repository.Repositories.StreamRepositories;
using StreamingProject.Repository.Repositories.SubscriptionRepositories;
using StreamingProject.Repository.Repositories.UserRepositories;
using StreamingProject.Repository.Repositories.VideoRepositories;

namespace StreamingProject.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStreamRepository, StreamRepositories>();
        services.AddScoped<IChatRepository, ChatRepositories>();
        services.AddScoped<IVideoRepository, VideoRepositories>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepositories>();
        
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        
        services.AddAsyncInitializer<DbInitializer>();

        return services;
    }
}