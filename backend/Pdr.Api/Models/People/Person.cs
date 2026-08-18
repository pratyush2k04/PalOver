using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Models.People;

public class Person : BaseAuditableEntity
{
    public int PersonId { get; set; }

    public int EngagementId { get; set; }

    public Engagement Engagement { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public PersonType PersonType { get; set; }
}
