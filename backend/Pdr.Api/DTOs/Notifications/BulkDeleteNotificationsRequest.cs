namespace Pdr.Api.DTOs.Notifications;

public class BulkDeleteNotificationsRequest
{
    public List<int> Ids { get; set; } = new();
}
