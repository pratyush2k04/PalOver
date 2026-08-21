using System;

namespace Pdr.Api.DTOs.Engagements;

public record DayOneSignOffRequest(
    DateOnly BillingStartDate,
    bool IsException = false,
    string? ExceptionJustification = null);
