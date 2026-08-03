namespace Pdr.Api.Models.EngagementAssignment;

using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;

public class EngagementAssignment
{
    public int UserId { get; set; }

    public int EngagementId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    public Engagement Engagement { get; set; } = null!;
}
