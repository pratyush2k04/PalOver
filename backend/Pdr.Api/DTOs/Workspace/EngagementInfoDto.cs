namespace Pdr.Api.DTOs.Workspace;

public record EngagementInfoDto(
    long EngagementId,
    string ClientName,
    string? EngagementDescription,
    string EngagementType,
    DateOnly KickoffDate,
    DateOnly? BillingStartDate,
    string Status);
