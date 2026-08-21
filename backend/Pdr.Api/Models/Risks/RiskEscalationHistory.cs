using System;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Risks;

public class RiskEscalationHistory : BaseAuditableEntity
{
    public int Id { get; set; }

    public int RiskId { get; set; }
    public DateTime EscalatedAt { get; set; } = DateTime.UtcNow;
    public RiskEscalationLevel EscalationLevel { get; set; }

    public int? EscalatedToUserId { get; set; }
    public string EscalatedToRole { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    // Navigation Properties
    public Risk Risk { get; set; } = null!;
    public User? EscalatedToUser { get; set; }
}
