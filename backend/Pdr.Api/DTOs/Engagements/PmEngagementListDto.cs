namespace Pdr.Api.DTOs.Engagements;

public record PmEngagementListItemDto(
    int EngagementId,
    string EngagementName,
    string ClientName,
    string EngagementTypeName,
    string CommercialModelName,
    DateOnly KickoffDate,
    int DaysToKickoff,
    int? ReadinessScore);

public record PmEngagementListResponse(List<PmEngagementListItemDto> Engagements);
