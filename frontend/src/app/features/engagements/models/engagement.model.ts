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
}

export interface EngagementOwners {
  projectManagerId?: number;
  projectManagerName?: string;
  techLeadId?: number;
  techLeadName?: string;
}

export interface PmEngagementListItem {
  engagementId: number;
  clientName: string;
  engagementTypeName: string;
  commercialModelName: string;
  kickoffDate: string;
  daysToKickoff: number;
  readinessScore: number | null;
}

export interface PmEngagementListResponse {
  engagements: PmEngagementListItem[];
}

export type EngagementSortBy = 'kickoff' | 'readiness' | 'client';
