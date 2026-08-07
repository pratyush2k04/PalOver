export enum AccessItemStatus {
  InProgress = 1,
  Granted = 2,
  Blocked = 3
}

export interface AccessItemDto {
  accessItemId: number;
  name: string;
  isClientSide: boolean;
  ownerId: number;
  ownerFullName: string;
  status: string;
  timeZoneNote?: string;
  expectedTurnaroundDays?: number;
  daysUntilBillingDate?: number;
  isOverdueWithClient: boolean;
  grantedDate?: string;
  blockReason?: string;
}

export interface AddCustomAccessItemRequest {
  name: string;
  isClientSide: boolean;
  timeZoneNote?: string;
  ownerId?: number;
}

export interface UpdateAccessItemStatusRequest {
  status: AccessItemStatus;
  blockReason?: string;
}

export interface UpdateAccessItemOwnerRequest {
  ownerId: number;
}
