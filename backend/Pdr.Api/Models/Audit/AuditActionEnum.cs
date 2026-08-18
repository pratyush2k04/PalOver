namespace Pdr.Api.Models.Audit;

public enum AuditActionEnum
{
    UserLoggedIn = 1,
    UserCreated = 2,
    UserUpdated = 3,
    UserDeleted = 4,
    HandoverQueryRaised = 856017,
    HandoverQueryResponded = 856018,
    HandoverQueryResolved = 856019
}
