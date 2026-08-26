using System.Collections.Generic;

namespace Pdr.Api.DTOs.ResourceRequests;

public class ResourceRequestDto
{
    public int Id { get; set; }
    public int EngagementId { get; set; }
    public string StandardRole { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = [];
    public int PeopleNeeded { get; set; }
    public DateOnly RequiredByDate { get; set; }
    public string JobDescription { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RequestedById { get; set; }
    public bool IsUrgent { get; set; }
    public List<ResourceAllocationDto> Allocations { get; set; } = [];
}
