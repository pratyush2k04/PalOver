namespace Pdr.Api.Models.EngagementAssignment;

using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;

public class EngagementAssignment
{
    public int UserId { get; set; }

    public int EngagementId { get; set; }

    // Navigation Properties
    [System.Text.Json.Serialization.JsonIgnore]
    public User User { get; set; } = null!;

    [System.Text.Json.Serialization.JsonIgnore]
    public Engagement Engagement { get; set; } = null!;
}
