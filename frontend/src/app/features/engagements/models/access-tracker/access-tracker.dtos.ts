import { AccessItemStatus } from './access-tracker.enums';

export interface AccessItemDto {
  id: number;
  name: string;
  isClientSide: boolean;
  ownerId: number;
  ownerFullName: string;
  status: string;
  timeZoneNote?: string;
  daysUntilBillingDate?: number;
  isOverdueWithClient: boolean;
  grantedDate?: string;
  blockReason?: string;
}
