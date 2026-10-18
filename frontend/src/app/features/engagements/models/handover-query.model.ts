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

export interface HandoverQueryMessageDto {
  id: number;
  queryId: number;
  senderUserId: number;
  senderName: string;
  senderRole: string;
  messageType: 'Question' | 'Response';
  messageText: string;
  sequenceNumber: number;
  createdAt: string;
}

export interface HandoverQueryDto {
  id: number;
  handoverId: number;
  fieldSection: HandoverQueryFieldSection;
  referenceId?: number;
  isResolved: boolean;
  raisedByName: string;
  raisedAt: string;
  resolvedByName?: string;
  resolvedByRole?: string;
  resolvedAt?: string;
  messages: HandoverQueryMessageDto[];
  latestMessageType: string;
  canAddFollowUp: boolean;
  canRespond: boolean;
}

export interface CreateHandoverQueryRequest {
  fieldSection: HandoverQueryFieldSection;
  referenceId?: number;
  question: string;
}

export interface AddHandoverQueryMessageRequest {
  messageText: string;
}
