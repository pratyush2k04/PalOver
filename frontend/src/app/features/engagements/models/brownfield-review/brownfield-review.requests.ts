import { BrownfieldSeverity, ClientDecision } from './brownfield-review.enums';

export interface CreateBrownfieldFindingRequest {
  title: string;
  description?: string;
  severity: BrownfieldSeverity;
  documentId?: number | null;
}

export interface UpdateClientDecisionRequest {
  clientDecision: ClientDecision;
}
