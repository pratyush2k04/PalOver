namespace Pdr.Api.Models.Engagements;

public class EngagementStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<Engagement> Engagements { get; set; } = [];
}
