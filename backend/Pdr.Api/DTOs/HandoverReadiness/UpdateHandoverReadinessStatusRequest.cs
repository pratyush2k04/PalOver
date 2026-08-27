using Pdr.Api.Models.HandoverReadiness;

namespace Pdr.Api.DTOs.HandoverReadiness;

public class UpdateHandoverReadinessStatusRequest
{
    public HandoverReadinessStatus Status { get; set; }
    public string? BlockReason { get; set; }
}
