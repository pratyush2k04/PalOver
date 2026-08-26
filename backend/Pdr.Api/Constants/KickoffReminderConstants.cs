using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Constants;

public static class KickoffReminderConstants
{
    public const string TemplateKey = "KickoffReadinessReminder";

    public static readonly IReadOnlyList<(int ThresholdDays, NotificationPriority Priority)> Thresholds =
    [
        (5, NotificationPriority.Low),
        (2, NotificationPriority.Medium),
        (1, NotificationPriority.High)
    ];

    public static readonly IReadOnlyList<RoleEnum> ReceiverRoles =
    [
        RoleEnum.PreSales,
        RoleEnum.ProjectManagement,
        RoleEnum.TechLead
    ];
}
