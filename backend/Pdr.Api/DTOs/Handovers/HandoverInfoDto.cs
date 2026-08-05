using System.Collections.Generic;

namespace Pdr.Api.DTOs.Handovers;

public class HandoverInfoDto
{
    public int HandoverId { get; set; }
    public int EngagementId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ClientBackground { get; set; }
    public string BusinessProblem { get; set; } = string.Empty;
    public string ClientSentiment { get; set; } = string.Empty;
    public string? DealOrigin { get; set; }
    public bool HasNoCommitments { get; set; }
    public List<CommitmentDto> Commitments { get; set; } = new();
}
