using System.Text.Json.Serialization;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.TechnicalReadiness;

public enum BrownfieldSeverity
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum ClientDecision
{
    AcceptNow = 1,
    DeferLater = 2
}

public class BrownfieldFinding : BaseAuditableEntity
{
    public int BrownfieldFindingId { get; set; }

    public int EngagementId { get; set; }

    [JsonIgnore]
    public Engagement Engagement { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public BrownfieldSeverity Severity { get; set; } = BrownfieldSeverity.Low;

    public ClientDecision? ClientDecision { get; set; }

    public int? DocumentId { get; set; }

    public Document? Document { get; set; }
}
