namespace Pdr.Api.Models.EngagementAssignment;

using Pdr.Api.Models.LoginEntity;
// using Pdr.Api.Models.Engagement; // Uncomment when your Engagement model is ready

public class EngagementAssignment
{
    public int UserId { get; set; }

    public int EngagementId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    // public Engagement Engagement { get; set; } = null!; // Add when Engagement entity exists
}
