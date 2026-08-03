namespace Pdr.Api.DTOs.Handovers;

public class CommitmentDto
{
    public long CommitmentId { get; set; }
    public long HandoverId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CommitmentTypeId { get; set; }
    public string? Quantity { get; set; }
}
