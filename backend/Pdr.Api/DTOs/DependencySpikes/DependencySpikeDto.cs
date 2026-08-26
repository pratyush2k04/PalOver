using Pdr.Api.Models.DependencySpikes;

namespace Pdr.Api.DTOs.DependencySpikes;

public class DependencySpikeDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public string DependencyName { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public DependencySpikeStatus Status { get; set; }
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? OutcomeFindings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
