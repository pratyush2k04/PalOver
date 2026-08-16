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

        // Add new mappers here when adding new audit action types.

        return services;
    }
}
