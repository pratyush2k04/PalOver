namespace Pdr.Api.Models.ResourceRequests;

public class ResourceRequestSkill
{
    public int ResourceRequestId { get; set; }

    public SkillEnum Skill { get; set; }

    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public ResourceRequest ResourceRequest { get; set; } = null!;
}
