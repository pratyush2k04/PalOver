using Pdr.Api.Models.Engagements.Dependencies;

namespace Pdr.Api.DTOs.Dependencies;

public class UpdateDependencyDto
{
    public string Description { get; set; } = string.Empty;
    public DependencyType Type { get; set; }
    public int OwnerId { get; set; }
    public DateTime NeededByDate { get; set; }
    public DependencyStatus Status { get; set; }
}
