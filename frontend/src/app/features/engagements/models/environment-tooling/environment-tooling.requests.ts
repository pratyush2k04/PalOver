import { EnvironmentToolingStatus } from './environment-tooling.enums';

export interface AddCustomEnvironmentToolingItemRequest {
  customName: string;
  ownerId?: number;
}

export interface UpdateEnvironmentToolingStatusRequest {
  status: EnvironmentToolingStatus;
  blockReason?: string;
}

export interface UpdateEnvironmentToolingOwnerRequest {
  ownerId: number;
}
