import { ArchitectureStatus } from './architecture-validation.enums';

export interface SaveArchitectureValidationRequest {
  status: ArchitectureStatus;
  rationale?: string;
}

export interface DiscoveryFeatureItemRequest {
  featureName: string;
  estimateWeeks: number;
  targetSprint: string;
}

// Aliases for backwards compatibility / semantic clarity
export type CreateDiscoveryFeatureItemRequest = DiscoveryFeatureItemRequest;
export type UpdateDiscoveryFeatureItemRequest = DiscoveryFeatureItemRequest;
