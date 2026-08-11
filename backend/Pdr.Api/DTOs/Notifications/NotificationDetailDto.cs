using Pdr.Api.Models.Notifications;

namespace Pdr.Api.DTOs.Notifications;

public record NotificationDetailDto(
    int Id,
    string Title,
    string Body,
    NotificationPriority Priority,
    string Status,
    DateTime ScheduledFor,
    DateTime? ReadAt,
    string? Link);
