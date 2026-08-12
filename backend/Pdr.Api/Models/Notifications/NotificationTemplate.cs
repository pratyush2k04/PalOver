namespace Pdr.Api.Models.Notifications;

public class NotificationTemplate : BaseAuditableEntity
{
    public int Id { get; set; }

    public required string Key { get; set; }

    public required string Title { get; set; }

    public required string Body { get; set; }

    public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Medium;
}
