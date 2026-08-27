using System;

namespace Pdr.Api.DTOs.Engagements;

public record EngagementInfoDto(
    int EngagementId,
    string EngagementName,
    string ClientName,
    string? EngagementDescription,
    string EngagementType,
    string CommercialModel,
    DateOnly KickoffDate,
    DateOnly? BillingStartDate,
    string Status,
    bool IsKickoffReady,
    string? HandoverStatus,
    DateTime? HandoverReviewedAt,
    string? HandoverReviewedByName,
    DayOneSignOffDto? DayOneSignOff = null);
