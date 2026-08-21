using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Models.Engagements;

public class Assumption : BaseAuditableEntity
{
    public int Id { get; set; }
    
    public int EngagementId { get; set; }
    public Engagement? Engagement { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public User? Owner { get; set; }

    public AssumptionStatus Status { get; set; } = AssumptionStatus.Unvalidated;

    public int? LinkedRiskId { get; set; }
    public Risk? LinkedRisk { get; set; }
}
