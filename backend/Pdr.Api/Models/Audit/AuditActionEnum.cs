namespace Pdr.Api.Models.Audit;

public enum AuditActionEnum
{
    UserLoggedIn = 1,
    UserCreated = 2,
    UserUpdated = 3,
    UserDeleted = 4,
    KickoffReminderScheduled = 19,
    KickoffReminderFired = 20,
    KickoffReminderSuppressed = 21,
    KickoffReminderReactivated = 22,
    KickoffReminderRetiredForBilling = 23
}
