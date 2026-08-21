using System;

namespace Pdr.Api.DTOs.Risks;

public class RiskEscalationHistoryDto
{
    public int Id { get; set; }
    public int RiskId { get; set; }
    public DateTime EscalatedAt { get; set; }
    public string EscalationLevel { get; set; } = string.Empty;
    public int? EscalatedToUserId { get; set; }
    public string? EscalatedToUserName { get; set; }
    public string EscalatedToRole { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
