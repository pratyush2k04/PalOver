using Pdr.Api.Models.Engagements;

namespace Pdr.Api.DTOs.Assumptions;

public class AssumptionDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public AssumptionStatus Status { get; set; }
}
