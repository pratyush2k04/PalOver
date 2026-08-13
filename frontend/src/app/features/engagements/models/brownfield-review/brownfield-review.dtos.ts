import { BrownfieldSeverity, ClientDecision } from './brownfield-review.enums';

export interface BrownfieldFindingDto {
  brownfieldFindingId: number;
  engagementId: number;
  title: string;
  description?: string;
  severity: BrownfieldSeverity;
  clientDecision?: ClientDecision | null;
  isDecisionNeeded: boolean;
  documentId?: number;
  documentName?: string;
  createdAt: string;
  createdBy?: string;
}

export interface BrownfieldReadinessResponse {
  engagementId: number;
  totalCount: number;
  highSeverityPendingCount: number;
  isTechnicallyReady: boolean;
  items: BrownfieldFindingDto[];
}
