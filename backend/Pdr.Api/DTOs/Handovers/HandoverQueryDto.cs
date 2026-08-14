namespace Pdr.Api.DTOs.Handovers;

public class HandoverQueryDto
{
    public int QueryId { get; set; }
    public int HandoverId { get; set; }
    public string FieldSection { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string RaisedByName { get; set; } = string.Empty;
    public string? RaisedByRole { get; set; }
    public DateTime RaisedAt { get; set; }
    public string? ResponseText { get; set; }
    public string? RespondedByName { get; set; }
    public string? RespondedByRole { get; set; }
    public DateTime? RespondedAt { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedByName { get; set; }
    public string? ResolvedByRole { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
