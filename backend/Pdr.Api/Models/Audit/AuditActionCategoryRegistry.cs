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
        [AuditActionEnum.ResourcingGapFlagRaised] = AuditCategoryEnum.Escalation,
        [AuditActionEnum.ResourcingGapFlagCleared] = AuditCategoryEnum.Escalation
    };
}
