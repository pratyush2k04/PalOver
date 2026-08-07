export enum EnvironmentToolingStatus {
  NotReady = 1,
  Ready = 2,
  NotApplicable = 3
}

export interface EnvironmentToolingItemDto {
  environmentToolingItemId: number;
  name: string;
  environmentToolingDefinitionId?: number;
  engagementId: number;
  ownerId: number;
  ownerFullName: string;
  customName?: string;
  status: EnvironmentToolingStatus;
  notApplicableReason?: string;
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

export interface AddCustomEnvironmentToolingItemRequest {
  customName: string;
  ownerId?: number;
}

export interface UpdateEnvironmentToolingStatusRequest {
  status: EnvironmentToolingStatus;
  notApplicableReason?: string;
  blockReason?: string;
}

export interface UpdateEnvironmentToolingOwnerRequest {
  ownerId: number;
}
