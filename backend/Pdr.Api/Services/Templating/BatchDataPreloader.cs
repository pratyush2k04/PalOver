using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services.Templating;

public class BatchDataPreloader(
    IUserRepository userRepository,
    IEngagementRepository engagementRepository,
    IDocumentRepository documentRepository,
    IDependencyRepository dependencyRepository,
    IRiskRepository riskRepository,
    IAccessTrackerRepository accessTrackerRepository,
    IResourceRequestRepository resourceRequestRepository,
    IHandoverQueryRepository handoverQueryRepository,
    IEngagementChecklistRepository checklistRepository,
    IKickoffRepository kickoffRepository,
    IRoleRepository roleRepository,
    IAssumptionRepository assumptionRepository,
    IServiceProvider serviceProvider) : IBatchDataPreloader
{
    public async Task PreloadAsync(BatchLoadContext context, CancellationToken cancellationToken = default)
    {
        // Avoid querying users already present in context (e.g. pre-seeded actor users)
        var missingUserIds = context.UserIds.Where(id => !context.Users.ContainsKey(id)).ToList();
        if (missingUserIds.Count > 0)
        {
            var users = await userRepository.GetByIdsAsync(missingUserIds);
            foreach (var u in users)
            {
                context.Users[u.Id] = u;
            }
        }

        var missingEngagementIds = context.EngagementIds.Where(id => !context.Engagements.ContainsKey(id)).ToList();
        if (missingEngagementIds.Count > 0)
        {
            var engagements = await engagementRepository.GetByIdsAsync(missingEngagementIds);
            foreach (var (k, v) in engagements)
            {
                context.Engagements[k] = v;
            }
        }

        var missingDocumentIds = context.DocumentIds.Where(id => !context.Documents.ContainsKey(id)).ToList();
        if (missingDocumentIds.Count > 0)
        {
            var docs = await documentRepository.GetByIdsAsync(missingDocumentIds);
            foreach (var (k, v) in docs)
            {
                context.Documents[k] = v;
            }
        }

        var missingCategoryIds = context.DocumentCategoryIds.Where(id => !context.DocumentCategories.ContainsKey(id)).ToList();
        if (missingCategoryIds.Count > 0)
        {
            var cats = await documentRepository.GetCategoriesByIdsAsync(missingCategoryIds);
            foreach (var (k, v) in cats)
            {
                context.DocumentCategories[k] = v;
            }
        }

        var missingDependencyIds = context.DependencyIds.Where(id => !context.Dependencies.ContainsKey(id)).ToList();
        if (missingDependencyIds.Count > 0)
        {
            var deps = await dependencyRepository.GetByIdsAsync(missingDependencyIds);
            foreach (var (k, v) in deps)
            {
                context.Dependencies[k] = v;
            }
        }

        var missingRiskIds = context.RiskIds.Where(id => !context.Risks.ContainsKey(id)).ToList();
        if (missingRiskIds.Count > 0)
        {
            var risks = await riskRepository.GetByIdsAsync(missingRiskIds);
            foreach (var (k, v) in risks)
            {
                context.Risks[k] = v;
            }
        }

        var missingAccessItemIds = context.AccessItemIds.Where(id => !context.AccessItems.ContainsKey(id)).ToList();
        if (missingAccessItemIds.Count > 0)
        {
            var items = await accessTrackerRepository.GetAccessItemsByIdsAsync(missingAccessItemIds);
            foreach (var (k, v) in items)
            {
                context.AccessItems[k] = v;
            }
        }

        var missingResourceRequestIds = context.ResourceRequestIds.Where(id => !context.ResourceRequests.ContainsKey(id)).ToList();
        if (missingResourceRequestIds.Count > 0)
        {
            var reqs = await resourceRequestRepository.GetByIdsAsync(missingResourceRequestIds);
            foreach (var (k, v) in reqs)
            {
                context.ResourceRequests[k] = v;
            }
        }

        var missingHandoverQueryIds = context.HandoverQueryIds.Where(id => !context.HandoverQueries.ContainsKey(id)).ToList();
        if (missingHandoverQueryIds.Count > 0)
        {
            var queries = await handoverQueryRepository.GetByIdsAsync(missingHandoverQueryIds, cancellationToken);
            foreach (var (k, v) in queries)
            {
                context.HandoverQueries[k] = v;
            }
        }

        var missingChecklistItemIds = context.ChecklistItemIds.Where(id => !context.ChecklistItems.ContainsKey(id)).ToList();
        if (missingChecklistItemIds.Count > 0)
        {
            var items = await checklistRepository.GetItemsByIdsAsync(missingChecklistItemIds, cancellationToken);
            foreach (var (k, v) in items)
            {
                context.ChecklistItems[k] = v;
            }
        }

        var missingActionItemIds = context.KickoffActionItemIds.Where(id => !context.KickoffActionItems.ContainsKey(id)).ToList();
        if (missingActionItemIds.Count > 0)
        {
            var actionItems = await kickoffRepository.GetActionItemsByIdsAsync(missingActionItemIds, cancellationToken);
            foreach (var (k, v) in actionItems)
            {
                context.KickoffActionItems[k] = v;
            }
        }

        var missingRoleIds = context.RoleIds.Where(id => !context.Roles.ContainsKey(id)).ToList();
        if (missingRoleIds.Count > 0)
        {
            var roles = await roleRepository.GetByIdsAsync(missingRoleIds, cancellationToken);
            foreach (var (k, v) in roles)
            {
                context.Roles[k] = v;
            }
        }

        var missingAssumptionIds = context.AssumptionIds.Where(id => !context.Assumptions.ContainsKey(id)).ToList();
        if (missingAssumptionIds.Count > 0)
        {
            var assumptions = await assumptionRepository.GetByIdsAsync(missingAssumptionIds, cancellationToken);
            foreach (var (k, v) in assumptions)
            {
                context.Assumptions[k] = v;
            }
        }

        if (context.KickoffReadinessEngagementIds.Count > 0)
        {
            var kickoffReadinessService = serviceProvider.GetRequiredService<IKickoffReadinessService>();
            foreach (var engId in context.KickoffReadinessEngagementIds)
            {
                if (!context.KickoffReadinessNextTabs.ContainsKey(engId))
                {
                    var readiness = await kickoffReadinessService.GetReadinessAsync(engId);
                    context.KickoffReadinessNextTabs[engId] = readiness.NextIncompleteTabKey ?? "handover";
                }
            }
        }
    }
}
