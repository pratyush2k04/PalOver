namespace Pdr.Api.DTOs.Engagements;

public record EngagementInfoDto(
    long EngagementId,
    string ClientName,
    string? EngagementDescription,
    string EngagementType,
    string CommercialModel,
    DateOnly KickoffDate,
    DateOnly? BillingStartDate,
    string Status);
