using Microsoft.Extensions.DependencyInjection;

namespace Pdr.Api.Services.Notifications;

// The single file devs touch to register a new notification template mapper.
public static class NotificationMapperRegistry
{
    public static IServiceCollection AddNotificationMappers(this IServiceCollection services)
    {
        // Add new mappers here when adding new notification types, e.g.:
        // services.AddScoped<INotificationTemplateMapper, AccessItemBlockedMapper>();

        return services;
    }
}
