namespace Pdr.Api.DTOs.Engagements;

public record EditEngagementRequest(
    string? EngagementName,
    string? ClientName,
    string? EngagementDescription,
    int? EngagementTypeId,
    int? CommercialModelId,
    DateOnly? KickoffDate,
    DateOnly? BillingStartDate);
