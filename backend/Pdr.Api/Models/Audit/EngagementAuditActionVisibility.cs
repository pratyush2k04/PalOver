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
        [AuditActionEnum.DocumentVersioned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DocumentUploaded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DocumentDeleted] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DocumentVisibilityChanged] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.MasterChecklistPublished] = new(VisibleToRoles: null),

        [AuditActionEnum.BrownfieldItemAdded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ClientPersonaChanged] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DayOneSummaryGenerated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DiscoveryItemAdded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.LeadershipAllocated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.PmAndTlAssigned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.PmAndTlReassigned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ResourcingGapFlagRaised] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ResourcingGapFlagCleared] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ChecklistItemEvidenceUpdated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DependencyCreated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DependencyUpdated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DependencyDeleted] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.DependencyEscalated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ReadinessOnePagerGenerated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.KickoffReadyMarked] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.KickoffReadyReverted] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.AccessItemBlocked] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.PersonAdded] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.ClientContactTransitioned] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.AssumptionCreated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.AssumptionUpdated] = new(VisibleToRoles: AllRoles),
        [AuditActionEnum.AssumptionValidated] = new(VisibleToRoles: AllRoles)
    };
}
