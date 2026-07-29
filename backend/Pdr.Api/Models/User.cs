namespace Pdr.Api.Models;

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
}
