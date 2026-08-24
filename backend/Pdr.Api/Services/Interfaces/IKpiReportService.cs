using Pdr.Api.DTOs.Kpi;

namespace Pdr.Api.Services.Interfaces;

public interface IKpiReportService
{
    Task<KpiReportResponseDto> GetReportAsync(KpiReportRequestDto request);
    Task<KpiReportFilterOptionsDto> GetFilterOptionsAsync();
}
