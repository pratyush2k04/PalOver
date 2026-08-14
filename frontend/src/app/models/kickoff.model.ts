export type KickoffActionItemStatus = 'Open' | 'InProgress' | 'Done';

export interface KickoffActionItem {
  kickoffActionItemId?: number;
  kickoffRecordId?: number;
  engagementId?: number;
  title: string;
  ownerName: string;
  assignedToUserId?: number | null;
  dueDate: string;
  status: KickoffActionItemStatus;
  isNotified?: boolean;
}

export interface KickoffRecord {
  kickoffRecordId: number;
  engagementId: number;
  outcomes: string;
  createdAt?: string;
  updatedAt?: string;
  actionItems: KickoffActionItem[];
}
