using System.Text.Json;
using System.Text.Json.Serialization;
using StreamingProject.Application;
using StreamingProject.Repository;

namespace StreamProject.Web;

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
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });
        services.AddOpenApi();
        services.AddSignalR();
        services.AddScoped<StreamingProject.Application.Interfaces.Chat.IChatNotificationService, StreamingProject.Presenters.Hubs.ChatNotificationService>();
        
        return services;
    }
}