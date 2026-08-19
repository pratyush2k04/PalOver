namespace Pdr.Api.Models.Audit;

/// <summary>
/// Declares, per <see cref="AuditActionEnum"/> value, which roles may see it in an engagement's
/// Audits feed. <c>VisibleToRoles: null</c> means the action isn't engagement-scoped content (the
/// role-matrix doesn't apply). Nothing reads this yet — it exists so the decision is captured in
/// one typed place as new actions are added, ready for the engagement Audits tab's visibility
/// matrix to consume later. See .agents/audit-log-skill.md.
/// </summary>
public record AuditActionVisibility(string[]? VisibleToRoles);

public static class AuditActionVisibilityRegistry
{
    public static readonly IReadOnlyDictionary<AuditActionEnum, AuditActionVisibility> Entries = new Dictionary<AuditActionEnum, AuditActionVisibility>
    {
        [AuditActionEnum.UserLoggedIn] = new(VisibleToRoles: null),
        [AuditActionEnum.UserCreated] = new(VisibleToRoles: null),
        [AuditActionEnum.UserUpdated] = new(VisibleToRoles: null),
        [AuditActionEnum.UserDeleted] = new(VisibleToRoles: null),
    };
}
