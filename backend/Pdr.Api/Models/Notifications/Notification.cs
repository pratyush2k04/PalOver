using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Notifications;

public class Notification : BaseAuditableEntity
{
    public int Id { get; set; }

    public int? TemplateId { get; set; }

    public NotificationTemplate? Template { get; set; }

    public int ReceiverUserId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public User Receiver { get; set; } = null!;

    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

    public DateTime? ReadAt { get; set; }

    public DateTime ScheduledFor { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public string? Data { get; set; }
}
