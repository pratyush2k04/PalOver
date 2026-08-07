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
    bool IsKickoffReady);
