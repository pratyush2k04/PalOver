export enum HandoverQueryFieldSection {
  ClientBackground = 'ClientBackground',
  BusinessProblem = 'BusinessProblem',
  ClientSentiment = 'ClientSentiment',
  DealOrigin = 'DealOrigin',
  Commitment = 'Commitment',
}

export const HANDOVER_SECTION_FRAGMENT_IDS: Record<HandoverQueryFieldSection, string> = {
  [HandoverQueryFieldSection.ClientBackground]: 'handover-client-background',
  [HandoverQueryFieldSection.BusinessProblem]: 'handover-business-problem',
  [HandoverQueryFieldSection.ClientSentiment]: 'handover-client-sentiment',
  [HandoverQueryFieldSection.DealOrigin]: 'handover-deal-origin',
  [HandoverQueryFieldSection.Commitment]: 'handover-commitment', // suffixed with -{referenceId}
};

export interface HandoverQueryDto {
  queryId: number;
  handoverId: number;
  fieldSection: HandoverQueryFieldSection;
  referenceId?: number;
  question: string;
  raisedByName: string;
  raisedByRole?: string;
  raisedAt: string;
  responseText?: string;
  respondedByName?: string;
  respondedByRole?: string;
  respondedAt?: string;
  isResolved: boolean;
  resolvedByName?: string;
  resolvedByRole?: string;
  resolvedAt?: string;
}

export interface CreateHandoverQueryRequest {
  fieldSection: HandoverQueryFieldSection;
  referenceId?: number;
  question: string;
}

export interface RespondHandoverQueryRequest {
  responseText: string;
}
