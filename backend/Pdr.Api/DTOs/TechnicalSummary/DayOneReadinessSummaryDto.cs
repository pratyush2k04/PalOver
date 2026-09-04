namespace Pdr.Api.DTOs.TechnicalSummary;

public class DayOneReadinessSummaryDto
{
    // Metadata
    public int EngagementId { get; set; }
    public string EngagementName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string EngagementType { get; set; } = string.Empty;
    public DateOnly DayOneDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public string? BriefDocumentName { get; set; }
    public string? BriefDocumentLink { get; set; }

    // Overall Status
    public bool IsAbleToStartDayOne { get; set; }
    public string OverallStatusMessage { get; set; } = string.Empty;

    // Access Readiness
    public int TotalAccessCount { get; set; }
    public int GrantedAccessCount { get; set; }
    public int InProgressAccessCount { get; set; }
    public int BlockedAccessCount { get; set; }
    public decimal AccessPercentReady { get; set; }
    public bool IsAccessReady { get; set; }

    // Environment & Tooling Readiness
    public int TotalToolingCount { get; set; }
    public int ReadyToolingCount { get; set; }
    public int NotReadyToolingCount { get; set; }
    public int BlockedToolingCount { get; set; }
    public decimal ToolingPercentReady { get; set; }
    public bool IsToolingReady { get; set; }

    // Architecture & Discovery Validation
    public string ArchitectureStatus { get; set; } = string.Empty;
    public string? ArchitectureRationale { get; set; }
    public decimal CommittedTimelineWeeks { get; set; }
    public decimal TotalEstimatedWeeks { get; set; }
    public decimal SizingDeltaWeeks { get; set; }
    public decimal SizingDeltaPercentage { get; set; }
    public bool IsSizingRiskRaised { get; set; }
    public string? RiskSummary { get; set; }
    public bool IsArchitectureReady { get; set; }

    // Brownfield Review
    public bool IsBrownfieldApplicable { get; set; }
    public int TotalBrownfieldFindings { get; set; }
    public int HighSeverityFindingsCount { get; set; }
    public int HighSeverityAcceptedCount { get; set; }
    public int HighSeverityPendingCount { get; set; }
    public bool IsBrownfieldReady { get; set; }

    // Blockers & Action Items
    public List<DayOneBlockerItemDto> OutstandingBlockers { get; set; } = [];
}
