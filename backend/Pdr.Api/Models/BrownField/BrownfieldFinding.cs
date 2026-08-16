using System.Text.Json.Serialization;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.BrownField;

public class BrownfieldFinding : BaseAuditableEntity
{
    public int Id { get; set; }

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
