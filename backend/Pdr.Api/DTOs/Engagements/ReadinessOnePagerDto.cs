namespace Pdr.Api.DTOs.Engagements;

public record ReadinessOnePagerDto(
    int EngagementId,
    string EngagementName,
    string ClientName,
    int RequesterId,
    string RequesterName,
    DateTime GeneratedAtUtc,
    int Score,
    int TotalApplicable,
    int TotalPassed,
    int HandoverFieldsApplicable,
    int HandoverFieldsPassed,
    int CommitmentsApplicable,
    int CommitmentsPassed,
    int ChecklistItemsApplicable,
    int ChecklistItemsPassed,
    DateOnly KickoffDate,
    DateOnly? BillingStartDate,
    int DaysToKickoff,
    string DaysToKickoffTier,
    int RequiredScoreThresholdForTier,
    string Status,
    List<OutstandingMandatoryItemDto> OutstandingMandatoryItems,
    List<OpenBlockerDto> OpenBlockers,
    List<CommitmentComparisonDto> Commitments,
    bool HasResourcingGap,
    int UnconfirmedResourceCount,
    int? DaysToBilling,
    bool IsDayOneSignedOff,
    bool DayOneSignOffIsException,
    CommercialDetailDto? Commercial,
    List<string> ScoreExclusionNotes);

public record OutstandingMandatoryItemDto(
    string Category,
    int? SourceId,
    string Label,
    string Reason);

public record OpenBlockerDto(
    int RiskId,
    string Title,
    string Status,
    string Impact,
    string? OwnerName);

public record CommitmentComparisonDto(
    int CommitmentId,
    string Title,
    string CommittedTimeline,
    string? RealisticTimeline,
    bool IsAcknowledged,
    string? AcknowledgementMessage);

public record CommercialDetailDto(
    string CommercialModelName,
    List<string> CommercialDocumentTitles);
