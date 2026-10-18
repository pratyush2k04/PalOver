import { EnvironmentToolingStatus } from './environment-tooling.enums';

export interface EnvironmentToolingItemDto {
  id: number;
  name: string;
  environmentToolingDefinitionId?: number;
  engagementId: number;
  ownerId: number;
  ownerFullName: string;
  customName?: string;
  status: EnvironmentToolingStatus;
  blockReason?: string;
}

export interface EnvironmentToolingReadinessResponse {
  engagementId: number;
  readyCount: number;
  totalCount: number;
  isTechnicallyReady: boolean;
  outstandingItems: EnvironmentToolingItemDto[];
  items: EnvironmentToolingItemDto[];
}
