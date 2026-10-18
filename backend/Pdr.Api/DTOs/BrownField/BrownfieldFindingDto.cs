using Pdr.Api.Models.BrownField;

namespace Pdr.Api.DTOs.BrownField;

public class BrownfieldFindingDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BrownfieldSeverity Severity { get; set; }
    public ClientDecision? ClientDecision { get; set; }
    public bool IsDecisionNeeded => Severity == BrownfieldSeverity.High && ClientDecision == null;
    public int? DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
