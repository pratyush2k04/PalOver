namespace Pdr.Api.DTOs.Engagements;

public record LeadershipEngagementListItemDto(
    int EngagementId,
    string EngagementName,
    string ClientName,
    string? PmName,
    int ReadinessScore,
    int OpenBlockerCount,
    int DaysToKickoff,
    string Status,
    bool HasResourcingGap,
    int UnconfirmedResourceCount,
    int? DaysToBilling,
    bool IsDayOneSignedOff = false,
    bool DayOneSignOffIsException = false);
