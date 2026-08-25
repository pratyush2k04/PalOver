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
        services.AddScoped<INotificationTemplateMapper, BlockerEscalatedPMMapper>();
        services.AddScoped<INotificationTemplateMapper, BlockerEscalatedLeadershipMapper>();
        services.AddScoped<INotificationTemplateMapper, RiskCommentMentionMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourceRequestRaisedMapper>();
        services.AddScoped<INotificationTemplateMapper, LeadershipAssignedMapper>();
        services.AddScoped<INotificationTemplateMapper, PMAssignedMapper>();
        services.AddScoped<INotificationTemplateMapper, TechLeadAssignedMapper>();
        services.AddScoped<INotificationTemplateMapper, DocumentUploadedMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourceRequestAllocatedMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourceRequestConfirmedMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourceRequestReopenedMapper>();
        services.AddScoped<INotificationTemplateMapper, ResourcingGapFlaggedNotificationMapper>();
        services.AddScoped<INotificationTemplateMapper, CommitmentVarianceEscalatedNotificationMapper>();
        services.AddScoped<INotificationTemplateMapper, HandoverDraftFirstSavedMapper>();

        // Add new mappers here when adding new notification types.

        return services;
    }
}
