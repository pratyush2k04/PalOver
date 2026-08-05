using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.DTOs.AccessTracker;

public class UpdateAccessItemStatusRequest
{
    public AccessItemStatus Status { get; set; }
    public string? BlockReason { get; set; }
}
