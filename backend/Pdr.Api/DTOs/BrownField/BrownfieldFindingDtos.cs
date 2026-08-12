using Pdr.Api.Models.BrownField;

namespace Pdr.Api.DTOs.BrownField;

public class BrownfieldFindingDto
{
    public int BrownfieldFindingId { get; set; }
    public int EngagementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BrownfieldSeverity Severity { get; set; }
    public ClientDecision? ClientDecision { get; set; }
    public bool IsDecisionNeeded => Severity == BrownfieldSeverity.High && ClientDecision == null;
    public int? DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateBrownfieldFindingRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public BrownfieldSeverity Severity { get; set; } = BrownfieldSeverity.Low;
    public int? DocumentId { get; set; }
}

public class UpdateClientDecisionRequest
{
    public ClientDecision ClientDecision { get; set; }
}

public class BrownfieldReadinessResponse
{
    public int EngagementId { get; set; }
    public int TotalCount { get; set; }
    public int HighSeverityPendingCount { get; set; }
    public bool IsTechnicallyReady { get; set; }
    public List<BrownfieldFindingDto> Items { get; set; } = new();
}
