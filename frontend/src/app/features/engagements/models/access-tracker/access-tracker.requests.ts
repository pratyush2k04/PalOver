import { AccessItemStatus } from './access-tracker.enums';

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
