using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.DTOs.AccessTracker;

public class AddCustomAccessItemRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsClientSide { get; set; }
    public int OwnerId { get; set; }
    public string? TimeZoneNote { get; set; }
}
