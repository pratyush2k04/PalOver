using Pdr.Api.DTOs.TechnicalSummary;

namespace Pdr.Api.Services.Interfaces;

public interface ITechnicalSummaryService
{
    Task<DayOneReadinessSummaryDto> GetDayOneSummaryAsync(int engagementId);
}
