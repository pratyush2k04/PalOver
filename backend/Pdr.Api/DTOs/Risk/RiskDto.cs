using Pdr.Api.Models.Risk;

namespace Pdr.Api.DTOs.Risk;

public class RiskDto
{
    public int RiskId { get; set; }
    public int EngagementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
