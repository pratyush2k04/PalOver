export enum BrownfieldSeverity {
  Low = 1,
  Medium = 2,
  High = 3
}

export enum ClientDecision {
  AcceptNow = 1,
  DeferLater = 2
}

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

export interface CreateBrownfieldFindingRequest {
  title: string;
  description?: string;
  severity: BrownfieldSeverity;
  documentId?: number | null;
}

export interface UpdateClientDecisionRequest {
  clientDecision: ClientDecision;
}

export interface BrownfieldReadinessResponse {
  engagementId: number;
  totalCount: number;
  highSeverityPendingCount: number;
  isTechnicallyReady: boolean;
  items: BrownfieldFindingDto[];
}
