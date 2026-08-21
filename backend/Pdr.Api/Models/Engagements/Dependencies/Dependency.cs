using Pdr.Api.Models.People;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Models.Engagements.Dependencies;

public class Dependency : BaseAuditableEntity
{
    public int Id { get; set; }
    
    public int EngagementId { get; set; }
    public Engagement Engagement { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    
    public DependencyType Type { get; set; }
    
    public int OwnerId { get; set; }
    public Person? Owner { get; set; }

    public DateTime NeededByDate { get; set; }
    
    public DependencyStatus Status { get; set; } = DependencyStatus.Open;

    public int? LinkedRiskId { get; set; }
    public Risk? LinkedRisk { get; set; }
}
