using Pdr.Api.Models.Notifications;

namespace Pdr.Api.DTOs.Notifications;

public record NotificationSummaryDto(
    int Id,
    string Title,
    string BodyPreview,
    NotificationPriority Priority,
    string Status,
    DateTime ScheduledFor);
