namespace Pdr.Api.Models.ResourceRequests;

public class ResourceAllocation : BaseAuditableEntity
{
    public int Id { get; set; }

    public int ResourceRequestId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly? ConfirmedDate { get; set; }

    public DateTime? RemovedAt { get; set; }

    public string? RemovalReason { get; set; }

    // Navigation properties
    [System.Text.Json.Serialization.JsonIgnore]
    public ResourceRequest ResourceRequest { get; set; } = null!;
}
