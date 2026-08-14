using System.Text.Json.Serialization;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Audit;

public class AuditLog : BaseAuditableEntity
{
    public int AuditLogId { get; set; }

    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public int AuditActionId { get; set; }

    [JsonIgnore]
    public AuditAction? AuditAction { get; set; }
}
