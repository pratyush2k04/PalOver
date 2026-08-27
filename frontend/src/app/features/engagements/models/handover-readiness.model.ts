export type HandoverReadinessStatus = 'InProgress' | 'Granted' | 'Blocked';

export interface HandoverReadinessItemDto {
  id: number;
  area: string;
  name: string;
  ownerFullName: string;
  status: HandoverReadinessStatus;
  blockReason?: string;
}

export interface AddCustomHandoverReadinessItemRequest {
  name: string;
}

export interface UpdateHandoverReadinessStatusRequest {
  status: HandoverReadinessStatus;
  blockReason?: string;
}

export interface AddHandoverItemDialogResult {
  name: string;
}

export interface BlockHandoverItemDialogData {
  itemName: string;
  currentReason: string;
}

export interface BlockHandoverItemDialogResult {
  reason: string;
}
