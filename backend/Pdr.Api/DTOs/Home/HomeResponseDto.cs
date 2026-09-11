using System;
using System.Collections.Generic;

namespace Pdr.Api.DTOs.Home;

public class HomeUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class HomeDashboardDto
{
    public List<DashboardCardsDto> Cards { get; set; } = [];
    public List<DashboardEngagementDto> UpcomingEngagements { get; set; } = [];
    public int TotalUpcomingEngagements { get; set; }
    public List<DashboardBlockerDto> CriticalBlockers { get; set; } = [];
    public int TotalCriticalBlockers { get; set; }
}

public class DashboardEngagementDto
{
    public int EngagementId { get; set; }
    public string EngagementName { get; set; } = string.Empty;
    public DateOnly KickoffDate { get; set; }
    public int DaysToGo { get; set; }
    public int ReadinessPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DashboardBlockerDto
{
    public int RiskId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string? Link { get; set; }
}

public class HomeResponseDto
{
    public HomeUserDto User { get; set; } = null!;
    public HomeDashboardDto Home { get; set; } = null!;
}
