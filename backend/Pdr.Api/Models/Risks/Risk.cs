using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Risks;

public class Risk : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public RiskImpact Impact { get; set; } = RiskImpact.Medium;
    public RiskLikelihood Likelihood { get; set; } = RiskLikelihood.Medium;

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public string? Mitigation { get; set; }

    public RiskStatus Status { get; set; } = RiskStatus.Open;

    public string? ResolutionNote { get; set; }

    // Source tracking for auto-created risks from blockers
    public string? SourceType { get; set; } // "Checklist", "AccessTracker", "EnvironmentTooling"
    public int? SourceItemId { get; set; }
    public string? SourceTitle { get; set; }
}
