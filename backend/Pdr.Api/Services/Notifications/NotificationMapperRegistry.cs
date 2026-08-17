using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services.Notifications;

// The single file devs touch to register a new notification template mapper.
public static class NotificationMapperRegistry
{
    public static IServiceCollection AddNotificationMappers(this IServiceCollection services)
    {
        services.AddScoped<INotificationTemplateMapper, AccessItemBlockedMapper>();
        services.AddScoped<INotificationTemplateMapper, PMAllocatedMapper>();
        services.AddScoped<INotificationTemplateMapper, HandoverSubmittedMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourceRequestRaisedMapper>();

        // Add new mappers here when adding new notification types.

        return services;
    }
}
