import { ArchitectureStatus } from './architecture-validation.enums';

export interface SaveArchitectureValidationRequest {
  status: ArchitectureStatus;
  rationale?: string;
}

export interface CreateDiscoveryFeatureItemRequest {
  featureName: string;
  estimateWeeks: number;
  targetSprint: string;
}

export interface UpdateDiscoveryFeatureItemRequest {
  featureName: string;
  estimateWeeks: number;
  targetSprint: string;
}
