using Pdr.Api.Models.Risks;

namespace Pdr.Api.DTOs.Risks;

public class CreateRiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskImpact Impact { get; set; } = RiskImpact.Medium;
    public RiskLikelihood Likelihood { get; set; } = RiskLikelihood.Medium;
    public int? OwnerId { get; set; }
    public string? Mitigation { get; set; }
}
