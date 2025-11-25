using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Service;
using StreamingProject.Application.Service.Chat;
using StreamingProject.Application.User;
using StreamingProject.Repository.Repositories;

namespace StreamingProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStreamRepository, StreamRepositories>();
        services.AddScoped<IChatRepository, ChatRepositories>();
        services.AddScoped<IVideoRepository, VideoRepositories>(); 
        
        return services;
    }
}