export interface MarkSowSignedRequest {
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
}

export interface Engagement {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
  statusId: number;
  checklistVersionId?: number;
}

export interface MarkSowSignedResponse {
  engagement: Engagement;
  warnings: string[];
}

export interface EngagementTypeOption {
  id: number;
  name: string;
}

export interface CommercialModelOption {
  id: number;
  name: string;
}

export interface GetEngagementTypesResponse {
  engagementTypes: EngagementTypeOption[];
}

export interface GetCommercialModelsResponse {
  commercialModels: CommercialModelOption[];
}

export interface DayOneSignOffRequest {
  billingStartDate: string;
  isException: boolean;
  exceptionJustification?: string;
}

export interface DayOneSignOffDto {
  billingStartDate: string;
  isException: boolean;
  exceptionJustification?: string;
  signedOffAt: string;
  signedOffByUserId: number;
  signedOffByUserName: string;
}

export interface EngagementInfo {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementType: string;
  commercialModel: string;
  kickoffDate: string;
  billingStartDate?: string;
  status: string;
  isKickoffReady: boolean;
  dayOneSignOff?: DayOneSignOffDto;
}

export interface EngagementInfoDto {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementType: string;
  kickoffDate: string;
  billingStartDate?: string;
  status: string;
  isKickoffReady: boolean;
  handoverStatus?: string;
  handoverReviewedAt?: string;
  handoverReviewedByName?: string;
  dayOneSignOff?: DayOneSignOffDto;
}

export interface EngagementOwners {
  projectManagerId?: number;
  projectManagerName?: string;
  techLeadId?: number;
  techLeadName?: string;
}

export interface EngagementListItem {
  id: number;
  engagementName: string;
  clientName: string;
  engagementTypeName: string;
  commercialModelName: string;
  kickoffDate: string;
  daysToKickoff: number;
  readinessScore: number | null;
  isSignedOff: boolean;
}

export interface EngagementListResponse {
  engagements: EngagementListItem[];
}

export type EngagementSortBy = 'kickoff' | 'readiness' | 'client';

export type OwnerKind = 'projectManager' | 'techLead';

export interface OwnerChange {
  kind: OwnerKind;
  label: string;
  userId: number;
  userName: string;
}
