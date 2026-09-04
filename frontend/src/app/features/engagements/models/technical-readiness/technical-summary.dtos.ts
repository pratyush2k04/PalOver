export interface DayOneBlockerItemDto {
  id: number;
  category: string;
  name: string;
  ownerName: string | null;
  status: string;
  detail: string | null;
  targetSection: string;
}

export interface DayOneReadinessSummaryDto {
  // Metadata
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementType: string;
  dayOneDate: string;
  generatedAt: string;
  generatedBy: string;
  briefDocumentName: string | null;
  briefDocumentLink: string | null;

  // Overall Status
  isAbleToStartDayOne: boolean;
  overallStatusMessage: string;

  // Access Readiness
  totalAccessCount: number;
  grantedAccessCount: number;
  inProgressAccessCount: number;
  blockedAccessCount: number;
  accessPercentReady: number;
  isAccessReady: boolean;

  // Environment & Tooling Readiness
  totalToolingCount: number;
  readyToolingCount: number;
  notReadyToolingCount: number;
  notApplicableToolingCount: number;
  applicableToolingCount: number;
  toolingPercentReady: number;
  isToolingReady: boolean;

  // Architecture & Discovery Validation
  architectureStatus: string;
  architectureRationale: string | null;
  committedTimelineWeeks: number;
  totalEstimatedWeeks: number;
  sizingDeltaWeeks: number;
  sizingDeltaPercentage: number;
  isSizingRiskRaised: boolean;
  riskSummary: string | null;
  isArchitectureReady: boolean;

  // Brownfield Review
  isBrownfieldApplicable: boolean;
  totalBrownfieldFindings: number;
  highSeverityFindingsCount: number;
  highSeverityAcceptedCount: number;
  highSeverityPendingCount: number;
  isBrownfieldReady: boolean;

  // Blockers & Action Items
  outstandingBlockers: DayOneBlockerItemDto[];
}
