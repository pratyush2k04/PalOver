namespace Pdr.Api.DTOs.Kpi;

public enum KpiReportDatePreset
{
    AllTime = 0,
    LastMonth = 1,
    Last6Months = 2,
    LastYear = 3,
    Custom = 4
}

public class KpiReportRequestDto
{
    public KpiReportDatePreset DatePreset { get; set; } = KpiReportDatePreset.AllTime;
    public DateOnly? CustomFrom { get; set; }
    public DateOnly? CustomTo { get; set; }
    public List<int>? PmUserIds { get; set; }
    public List<int>? EngagementTypeIds { get; set; }
}
