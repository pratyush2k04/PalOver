using Pdr.Api.Models.Risks;

namespace Pdr.Api.DTOs.Risks;

public class UpdateRiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskImpact Impact { get; set; }
    public RiskLikelihood Likelihood { get; set; }
    public int? OwnerId { get; set; }
    public string? Mitigation { get; set; }
    public RiskStatus Status { get; set; } = RiskStatus.Open;
}
