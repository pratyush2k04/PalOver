using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.Engagements.Dependencies;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.Kickoff;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Services.Templating;

public class BatchLoadContext
{
    // ID collections for Phase 1 (Collector)
    public HashSet<int> UserIds { get; } = [];
    public HashSet<int> EngagementIds { get; } = [];
    public HashSet<int> DocumentIds { get; } = [];
    public HashSet<int> DocumentCategoryIds { get; } = [];
    public HashSet<int> DependencyIds { get; } = [];
    public HashSet<int> RiskIds { get; } = [];
    public HashSet<int> AccessItemIds { get; } = [];
    public HashSet<int> ResourceRequestIds { get; } = [];
    public HashSet<int> HandoverQueryIds { get; } = [];
    public HashSet<int> ChecklistItemIds { get; } = [];
    public HashSet<int> KickoffActionItemIds { get; } = [];
    public HashSet<int> RoleIds { get; } = [];
    public HashSet<int> AssumptionIds { get; } = [];
    public HashSet<int> KickoffReadinessEngagementIds { get; } = [];

    // Preloaded entity dictionaries for Phase 3 (Mapper)
    public Dictionary<int, User> Users { get; set; } = [];
    public Dictionary<int, Engagement> Engagements { get; set; } = [];
    public Dictionary<int, Document> Documents { get; set; } = [];
    public Dictionary<int, DocumentCategory> DocumentCategories { get; set; } = [];
    public Dictionary<int, Dependency> Dependencies { get; set; } = [];
    public Dictionary<int, Risk> Risks { get; set; } = [];
    public Dictionary<int, AccessItem> AccessItems { get; set; } = [];
    public Dictionary<int, ResourceRequest> ResourceRequests { get; set; } = [];
    public Dictionary<int, HandoverQuery> HandoverQueries { get; set; } = [];
    public Dictionary<int, EngagementChecklistItem> ChecklistItems { get; set; } = [];
    public Dictionary<int, KickoffActionItem> KickoffActionItems { get; set; } = [];
    public Dictionary<int, Role> Roles { get; set; } = [];
    public Dictionary<int, Assumption> Assumptions { get; set; } = [];
    public Dictionary<int, string?> KickoffReadinessNextTabs { get; set; } = [];
}
