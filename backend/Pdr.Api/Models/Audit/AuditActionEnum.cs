namespace Pdr.Api.Models.Audit;

public enum AuditActionEnum
{
    UserLoggedIn = 1,
    UserCreated = 2,
    UserUpdated = 3,
    UserDeleted = 4,
    DocumentVersioned = 5,
    DocumentUploaded = 6,
    UnauthorizedAccess = 7,
    AccountLockout = 8,
    ClientContactDetailsChanged = 9,
    ClientPersonaChanged = 10,
    LeadershipAllocated = 11,
    UserRoleAllocated = 12,
    PmAndTlAssigned = 13,
    PmAndTlReassigned = 14,
    DayOneSummaryGenerated = 15,
    MasterChecklistPublished = 16,
    BrownfieldItemAdded = 17,
    DiscoveryItemAdded = 18,
    ResourcingGapFlagRaised = 19,
    ResourcingGapFlagCleared = 20,
    ChecklistItemEvidenceUpdated = 21,
    DependencyCreated = 22,
    DependencyUpdated = 23,
    DependencyDeleted = 24,
    DependencyEscalated = 25,
    EvidenceSnapshotCreated = 26

}
