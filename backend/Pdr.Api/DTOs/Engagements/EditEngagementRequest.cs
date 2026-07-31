namespace Pdr.Api.DTOs.Engagements;

public record EditEngagementRequest(
    string? ClientName,
    string? EngagementDescription,
    int? EngagementTypeId,
    DateOnly? KickoffDate,
    DateOnly? BillingStartDate);
