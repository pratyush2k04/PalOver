namespace Pdr.Api.DTOs.Handovers;

public class HandoverQueryMessageDto
{
    public int Id { get; set; }
    public int QueryId { get; set; }
    public int SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
