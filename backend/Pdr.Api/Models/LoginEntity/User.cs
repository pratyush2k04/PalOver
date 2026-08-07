namespace Pdr.Api.Models.LoginEntity;

using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EngagementAssignment;

public class User : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public int FailedAttemptCount { get; set; } = 0;

    public DateTimeOffset? LockoutEnd { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<EngagementAssignment> EngagementAssignments { get; set; } = new List<EngagementAssignment>();

    //Technical Readiness
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<AccessItem> AccessItems { get; set; }
    = new List<AccessItem>();

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<EnvironmentToolingItem> EnvironmentToolingItems { get; set; }
        = new List<EnvironmentToolingItem>();
}
