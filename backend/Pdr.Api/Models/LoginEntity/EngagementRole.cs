namespace Pdr.Api.Models.LoginEntity;

using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;

public class EngagementRole : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<EngagementAssignment> EngagementAssignments { get; set; } = new List<EngagementAssignment>();
}
