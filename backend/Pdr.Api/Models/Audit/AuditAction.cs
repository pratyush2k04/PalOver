namespace Pdr.Api.Models.Audit;

public class AuditAction
{
    public int Id { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
