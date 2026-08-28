using Pdr.Api.Models.Engagements.Dependencies;

namespace Pdr.Api.DTOs.Dependencies;

public class DependencyDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DependencyType Type { get; set; }
    public int? OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int? OwnerUserId { get; set; }
    public DateTime NeededByDate { get; set; }
    public DependencyStatus Status { get; set; }
    public int? LinkedRiskId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}