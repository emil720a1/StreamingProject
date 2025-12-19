using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Service;
using StreamingProject.Application.Service.Chat;
using StreamingProject.Application.Service.Chat.CharService;
using StreamingProject.Application.Service.Permission.PermissionService;
using StreamingProject.Application.Service.Role.RoleService;
using StreamingProject.Application.Service.Stream.StreamService;
using StreamingProject.Application.Service.Subscription.SubscriptionService;
using StreamingProject.Application.Service.User.UserService;
using StreamingProject.Application.Service.Video;
using StreamingProject.Contracts.User;

namespace StreamingProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.AddScoped<IStreamService, StreamService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IVideoService, VideoService>();
        
        return services;
    }
}