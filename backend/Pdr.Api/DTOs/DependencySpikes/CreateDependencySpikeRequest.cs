using Pdr.Api.Models.DependencySpikes;

namespace Pdr.Api.DTOs.DependencySpikes;

public class CreateDependencySpikeRequest
{
    public required string DependencyName { get; set; }
    public required string Objective { get; set; }
    public DependencySpikeStatus Status { get; set; } = DependencySpikeStatus.InProgress;
    public int? OwnerId { get; set; }
    public string? OutcomeFindings { get; set; }
}
