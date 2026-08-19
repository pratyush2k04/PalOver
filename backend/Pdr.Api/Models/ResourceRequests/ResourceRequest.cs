using System.Collections.Generic;

namespace Pdr.Api.Models.ResourceRequests;

public class ResourceRequest : BaseAuditableEntity
{
    public int Id { get; set; }

    public int EngagementId { get; set; }

    public StandardRoleEnum StandardRole { get; set; }

    public int PeopleNeeded { get; set; }

    public DateOnly RequiredByDate { get; set; }

    public string JobDescription { get; set; } = string.Empty;

    public ResourceRequestStatus Status { get; set; } = ResourceRequestStatus.Raised;

    public int RequestedById { get; set; }

    // Navigation properties
    public ICollection<ResourceRequestSkill> RequestSkills { get; set; } = new List<ResourceRequestSkill>();
}
