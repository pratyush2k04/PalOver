using Pdr.Api.Constants;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Audit;

/// <summary>
/// Declares, per <see cref="AuditActionEnum"/> value, which roles may see it in an engagement's
/// Audits feed. <c>VisibleToRoles: null</c> means the action isn't engagement-scoped content (the
/// role-matrix doesn't apply). 
/// </summary>
public record EngagementAuditActionVisibility(RoleEnum[]? VisibleToRoles);

public static class EngagementAuditActionVisibilityRegistry
{
    private static readonly RoleEnum[] AllRoles = 
    [
        RoleEnum.PreSales, 
        RoleEnum.ProjectManagement, 
        RoleEnum.TechLead, 
        RoleEnum.Leadership
    ];

    public static readonly IReadOnlyDictionary<AuditActionEnum, EngagementAuditActionVisibility> Entries = new Dictionary<AuditActionEnum, EngagementAuditActionVisibility>
    {
        [AuditActionEnum.UserLoggedIn] = new(VisibleToRoles: null),
        [AuditActionEnum.UserCreated] = new(VisibleToRoles: null),
        [AuditActionEnum.UserUpdated] = new(VisibleToRoles: null),
        [AuditActionEnum.UserDeleted] = new(VisibleToRoles: null),
        [AuditActionEnum.UnauthorizedAccess] = new(VisibleToRoles: null),
        [AuditActionEnum.AccountLockout] = new(VisibleToRoles: null),
        [AuditActionEnum.UserRoleAllocated] = new(VisibleToRoles: null),
        [AuditActionEnum.DocumentVersioned] = new(VisibleToRoles: null),
        [AuditActionEnum.DocumentUploaded] = new(VisibleToRoles: null),
        [AuditActionEnum.MasterChecklistPublished] = new(VisibleToRoles: null),

        [AuditActionEnum.BrownfieldItemAdded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ClientContactDetailsChanged] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ClientPersonaChanged] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DayOneSummaryGenerated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DiscoveryItemAdded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.LeadershipAllocated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.PmAndTlAssigned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.PmAndTlReassigned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ResourcingGapFlagRaised] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ResourcingGapFlagCleared] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ChecklistItemEvidenceUpdated] = new(VisibleToRoles: AllRoles)
    };
}
