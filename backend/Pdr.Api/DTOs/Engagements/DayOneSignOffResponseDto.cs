using System;

namespace Pdr.Api.DTOs.Engagements;

public record DayOneSignOffResponseDto(
    int EngagementId,
    int SignedOffByUserId,
    string SignerName,
    DateOnly BillingStartDate,
    DateTime SignedOffAt,
    bool IsException,
    string? ExceptionJustification);
