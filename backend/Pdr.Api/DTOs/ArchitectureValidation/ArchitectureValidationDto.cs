using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.DTOs.ArchitectureValidation;

public class ArchitectureValidationDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public ArchitectureStatus Status { get; set; }
    public string? Rationale { get; set; }
    public decimal CommittedTimelineWeeks { get; set; }
    public decimal TotalEstimatedWeeks { get; set; }
    public decimal SizingDeltaWeeks { get; set; }
    public decimal SizingDeltaPercentage { get; set; }
    public bool IsRiskRaised { get; set; }
    public string? RiskSummary { get; set; }
    public List<DiscoveryFeatureItemDto> Features { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
