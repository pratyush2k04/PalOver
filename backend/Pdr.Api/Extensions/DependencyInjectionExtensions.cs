using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Repositories;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;

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
        services.AddScoped<IChangeRequestRepository, ChangeRequestRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<IHandoverAccessService, HandoverAccessService>();
        services.AddScoped<IHandoverService, HandoverService>();
        services.AddScoped<ICommitmentService, CommitmentService>();
        services.AddScoped<IRealisticCommitmentService, RealisticCommitmentService>();
        services.AddScoped<IMasterChecklistService, MasterChecklistService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ICommunicationCadenceService, CommunicationCadenceService>();
        services.AddScoped<IEngagementChecklistService, EngagementChecklistService>();
        services.AddScoped<IAccessTrackerService, AccessTrackerService>();
        services.AddScoped<IEnvironmentToolingService, EnvironmentToolingService>();
        services.AddScoped<IChangeRequestService, ChangeRequestService>();
        services.AddScoped<IPersonService, PersonService>();

        return services;
    }
}
