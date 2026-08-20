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
    bool IsDayOneSignedOff = false,
    bool DayOneSignOffIsException = false,
    string? DayOneSignOffSignerName = null,
    DateTime? DayOneSignedOffAt = null,
    DateOnly? DayOneSignOffBillingDate = null,
    string? DayOneSignOffExceptionJustification = null);
