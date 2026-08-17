using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Repositories;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services;
using Pdr.Api.Services.Audit;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IEngagementRepository, EngagementRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IHandoverRepository, HandoverRepository>();
        services.AddScoped<ICommitmentRepository, CommitmentRepository>();
        services.AddScoped<IRealisticCommitmentRepository, RealisticCommitmentRepository>();
        services.AddScoped<IMasterChecklistRepository, MasterChecklistRepository>();
        services.AddScoped<ICommunicationCadenceRepository, CommunicationCadenceRepository>();
        services.AddScoped<IEngagementChecklistRepository, EngagementChecklistRepository>();
        services.AddScoped<IAccessTrackerRepository, AccessTrackerRepository>();
        services.AddScoped<IEnvironmentToolingRepository, EnvironmentToolingRepository>();
        services.AddScoped<IBrownfieldFindingRepository, BrownfieldFindingRepository>();
        services.AddScoped<IChangeRequestRepository, ChangeRequestRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IClientContactTransitionRepository, ClientContactTransitionRepository>();
        services.AddScoped<IRiskRepository, RiskRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IArchitectureValidationRepository, ArchitectureValidationRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<IHandoverAccessService, HandoverAccessService>();
        services.AddScoped<IHandoverService, HandoverService>();
        services.AddScoped<ICommitmentService, CommitmentService>();
        services.AddScoped<IRealisticCommitmentService, RealisticCommitmentService>();
        services.AddScoped<IKickoffReadinessService, KickoffReadinessService>();
        services.AddScoped<IKickoffReminderService, KickoffReminderService>();
        services.AddScoped<IMasterChecklistService, MasterChecklistService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ICommunicationCadenceService, CommunicationCadenceService>();
        services.AddScoped<IEngagementChecklistService, EngagementChecklistService>();
        services.AddScoped<IAccessTrackerService, AccessTrackerService>();
        services.AddScoped<IEnvironmentToolingService, EnvironmentToolingService>();
        services.AddScoped<IBrownfieldFindingService, BrownfieldFindingService>();
        services.AddScoped<IChangeRequestService, ChangeRequestService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IClientContactTransitionService, ClientContactTransitionService>();
        services.AddScoped<IRiskService, RiskService>();
        services.AddScoped<IKickoffRepository, KickoffRepository>();
        services.AddScoped<IKickoffService, KickoffService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IArchitectureValidationService, ArchitectureValidationService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<TemplateMapperResolver<INotificationTemplateMapper>>();
        services.AddScoped<TemplateMapperResolver<IAuditTemplateMapper>>();
        services.AddSingleton<TemplateRenderer>();

        return services;
    }
}
