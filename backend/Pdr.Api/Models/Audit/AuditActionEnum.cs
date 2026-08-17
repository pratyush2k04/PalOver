namespace Pdr.Api.Models.Audit;

public enum AuditActionEnum
{
    UserLoggedIn = 1,
    UserCreated = 2,
    UserUpdated = 3,
    UserDeleted = 4,
    KickoffReminderScheduled = 5,
    KickoffReminderFired = 6,
    KickoffReminderSuppressed = 7,
    KickoffReminderReactivated = 8,
    KickoffReminderRetiredForBilling = 9
}
