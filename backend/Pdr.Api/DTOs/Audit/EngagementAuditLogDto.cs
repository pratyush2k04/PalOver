namespace Pdr.Api.DTOs.Audit;

public class EngagementAuditLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
