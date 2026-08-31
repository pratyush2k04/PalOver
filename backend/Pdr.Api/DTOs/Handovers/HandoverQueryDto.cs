namespace Pdr.Api.DTOs.Handovers;

public class HandoverQueryDto
{
    public int Id { get; set; }
    public int HandoverId { get; set; }
    public string FieldSection { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public bool IsResolved { get; set; }
    public string RaisedByName { get; set; } = string.Empty;
    public DateTime RaisedAt { get; set; }
    public string? ResolvedByName { get; set; }
    public string? ResolvedByRole { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<HandoverQueryMessageDto> Messages { get; set; } = [];
    public string LatestMessageType { get; set; } = string.Empty;
    public bool CanAddFollowUp { get; set; }
    public bool CanRespond { get; set; }
}
