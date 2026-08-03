namespace Pdr.Api.DTOs.Handovers;

public class UpdateCommitmentRequest
{
    public string Description { get; set; } = string.Empty;
    public int CommitmentTypeId { get; set; }
    public string? Quantity { get; set; }
}
