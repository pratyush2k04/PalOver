using Pdr.Api.DTOs.Common;

namespace Pdr.Api.DTOs.Engagements;

public record EngagementListItemDto(
    int Id,
    string EngagementName,
    string ClientName,
    string EngagementTypeName,
    string CommercialModelName,
    DateOnly KickoffDate,
    int DaysToKickoff,
    int? ReadinessScore,
    bool IsSignedOff);

public record EngagementListResponse(List<EngagementListItemDto> Engagements);

public class MyEngagementsPageDto : PagedResult<EngagementListItemDto>
{
    public int ActiveCount { get; set; }
    public int PastCount { get; set; }
}
