namespace Pdr.Api.Models.LoginEntity;

using Pdr.Api.Models;
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

    public ICollection<EngagementAssignment> EngagementAssignments { get; set; } = new List<EngagementAssignment>();
}
