import { ArchitectureStatus } from './architecture-validation.enums';

export interface DiscoveryFeatureItemDto {
  id: number;
  architectureValidationId: number;
  featureName: string;
  estimateWeeks: number;
  targetSprint: string;
  createdAt: string;
  createdBy?: string;
}

export interface ArchitectureValidationDto {
  id: number;
  engagementId: number;
  status: ArchitectureStatus;
  rationale?: string;
  committedTimelineWeeks: number;
  totalEstimatedWeeks: number;
  sizingDeltaWeeks: number;
  sizingDeltaPercentage: number;
  isRiskRaised: boolean;
  riskSummary?: string;
  features: DiscoveryFeatureItemDto[];
  updatedAt: string;
  updatedBy?: string;
}
