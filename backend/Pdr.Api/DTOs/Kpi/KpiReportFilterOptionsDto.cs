using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.DTOs.Kpi;

public record KpiReportFilterOptionsDto(List<SimpleUserDto> AvailablePms, List<EngagementTypeDto> AvailableEngagementTypes);
