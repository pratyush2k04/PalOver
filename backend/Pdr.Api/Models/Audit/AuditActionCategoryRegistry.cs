namespace Pdr.Api.Models.Audit;

public static class AuditActionCategoryRegistry
{
    public static readonly IReadOnlyDictionary<AuditActionEnum, AuditCategoryEnum> Entries = new Dictionary<AuditActionEnum, AuditCategoryEnum>
    {
        [AuditActionEnum.DocumentVersioned] = AuditCategoryEnum.Document,
        [AuditActionEnum.DocumentUploaded] = AuditCategoryEnum.Document,
        [AuditActionEnum.MasterChecklistPublished] = AuditCategoryEnum.Checklist,
        [AuditActionEnum.UnauthorizedAccess] = AuditCategoryEnum.Access,
        [AuditActionEnum.AccountLockout] = AuditCategoryEnum.Access,
        [AuditActionEnum.UserLoggedIn] = AuditCategoryEnum.Access,
        [AuditActionEnum.UserCreated] = AuditCategoryEnum.Access,
        [AuditActionEnum.UserUpdated] = AuditCategoryEnum.Access,
        [AuditActionEnum.UserDeleted] = AuditCategoryEnum.Access,
        [AuditActionEnum.ResourcingGapFlagRaised] = AuditCategoryEnum.Escalation,
        [AuditActionEnum.ResourcingGapFlagCleared] = AuditCategoryEnum.Escalation,
        [AuditActionEnum.ClientContactDetailsChanged] = AuditCategoryEnum.Handover,
        [AuditActionEnum.ClientPersonaChanged] = AuditCategoryEnum.Handover,
        [AuditActionEnum.LeadershipAllocated] = AuditCategoryEnum.ResourceAllocation,
        [AuditActionEnum.UserRoleAllocated] = AuditCategoryEnum.ResourceAllocation,
        [AuditActionEnum.PmAndTlAssigned] = AuditCategoryEnum.ResourceAllocation,
        [AuditActionEnum.PmAndTlReassigned] = AuditCategoryEnum.ResourceAllocation,
        [AuditActionEnum.DayOneSummaryGenerated] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.BrownfieldItemAdded] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.DiscoveryItemAdded] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.ChecklistItemEvidenceUpdated] = AuditCategoryEnum.Checklist,
        [AuditActionEnum.DependencyCreated] = AuditCategoryEnum.Risk,
        [AuditActionEnum.DependencyUpdated] = AuditCategoryEnum.Risk,
        [AuditActionEnum.DependencyDeleted] = AuditCategoryEnum.Risk,
        [AuditActionEnum.DependencyEscalated] = AuditCategoryEnum.Escalation,
        [AuditActionEnum.ReadinessOnePagerGenerated] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.KickoffReadyMarked] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.KickoffReadyReverted] = AuditCategoryEnum.TechnicalReadiness,
        [AuditActionEnum.AccessItemBlocked] = AuditCategoryEnum.TechnicalReadiness
    };
}
