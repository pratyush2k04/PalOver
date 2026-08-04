namespace Pdr.Api.DTOs.Engagements;

public record MarkSowSignedRequest(
    string EngagementName,
    string ClientName,
    string EngagementDescription,
    int EngagementTypeId,
    int CommercialModelId,
    DateOnly KickoffDate,
    DateOnly BillingStartDate);
