using Pdr.Api.Models.DependencySpikes;

namespace Pdr.Api.DTOs.DependencySpikes;

public class UpdateDependencySpikeRequest
{
    public required string DependencyName { get; set; }
    public required string Objective { get; set; }
    public DependencySpikeStatus Status { get; set; }
    public int? OwnerId { get; set; }
    public string? OutcomeFindings { get; set; }
}
