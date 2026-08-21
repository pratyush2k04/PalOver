using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Services.Audit.Mappers;

namespace Pdr.Api.Services.Audit;

// The single file devs touch to register a new audit action mapper.
public static class AuditMapperRegistry
{
    public static IServiceCollection AddAuditMappers(this IServiceCollection services)
    {
        services.AddScoped<IAuditTemplateMapper, UserLoggedInMapper>();
        services.AddScoped<IAuditTemplateMapper, UserCreatedMapper>();
        services.AddScoped<IAuditTemplateMapper, UserUpdatedMapper>();
        services.AddScoped<IAuditTemplateMapper, UserDeletedMapper>();
        services.AddScoped<IAuditTemplateMapper, UnauthorizedAccessMapper>();
        services.AddScoped<IAuditTemplateMapper, AccountLockoutMapper>();
        services.AddScoped<IAuditTemplateMapper, DocumentVersionedMapper>();
        services.AddScoped<IAuditTemplateMapper, DocumentUploadedMapper>();
        services.AddScoped<IAuditTemplateMapper, ClientContactDetailsChangedMapper>();
        services.AddScoped<IAuditTemplateMapper, ClientPersonaChangedMapper>();
        services.AddScoped<IAuditTemplateMapper, LeadershipAllocatedMapper>();
        services.AddScoped<IAuditTemplateMapper, UserRoleAllocatedMapper>();
        services.AddScoped<IAuditTemplateMapper, PmAndTlAssignedMapper>();
        services.AddScoped<IAuditTemplateMapper, PmAndTlReassignedMapper>();
        services.AddScoped<IAuditTemplateMapper, MasterChecklistPublishedMapper>();
        services.AddScoped<IAuditTemplateMapper, DayOneSummaryGeneratedMapper>();
        services.AddScoped<IAuditTemplateMapper, BrownfieldItemAddedMapper>();
        services.AddScoped<IAuditTemplateMapper, DiscoveryItemAddedMapper>();
        services.AddScoped<IAuditTemplateMapper, ResourcingGapFlagRaisedMapper>();
        services.AddScoped<IAuditTemplateMapper, ResourcingGapFlagClearedMapper>();
        services.AddScoped<IAuditTemplateMapper, ChecklistItemEvidenceUpdatedMapper>();

        // Add new mappers here when adding new audit action types.

        return services;
    }
}
