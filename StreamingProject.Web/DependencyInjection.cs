using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StreamingProject.Application.Service;
using StreamingProject.Application.Service.Chat;
using StreamingProject.Application.User;
using StreamingProject.Repository.Repositories;

namespace StreamingProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {

        services.AddWebDependencies();
        services.AddApplication();
        services.AddInfrastructure();
        
        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        
        return services;
    }
}