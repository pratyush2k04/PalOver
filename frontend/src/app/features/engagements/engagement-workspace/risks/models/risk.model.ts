export type RiskImpact = 'Low' | 'Medium' | 'High';
export type RiskLikelihood = 'Low' | 'Medium' | 'High';
export type RiskStatus = 'Open' | 'Mitigating' | 'Closed';

export interface OwnerOption {
  id: number;
  name: string;
  role?: string;
}

export interface Risk {
  id: number;
  title: string;
  description: string;
  impact: RiskImpact;
  likelihood: RiskLikelihood;
  ownerId?: number | null;
  ownerName?: string | null;
  mitigation?: string | null;
  status: RiskStatus;
  resolutionNote?: string | null;
  openForDays: number;
  sourceType?: string | null;
  sourceItemId?: number | null;
  sourceTitle?: string | null;
}

export interface CreateRiskRequest {
  title: string;
  description: string;
  impact: RiskImpact;
  likelihood: RiskLikelihood;
  ownerId?: number | null;
  mitigation?: string | null;
}

export interface UpdateRiskRequest {
  title: string;
  description: string;
  impact: RiskImpact;
  likelihood: RiskLikelihood;
  ownerId?: number | null;
  mitigation?: string | null;
  status: RiskStatus;
}

export interface CloseRiskRequest {
  resolutionNote: string;
}

export interface RiskComment {
  id: number;
  riskId: number;
  authorId: number;
  authorName: string;
  content: string;
  isEdited: boolean;
  canEdit: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRiskCommentRequest {
  content: string;
  mentionedUserIds?: number[];
}

export interface UpdateRiskCommentRequest {
  content: string;
}

export interface EngagementMember {
  id: number;
  fullName: string;
  email: string;
  roleName?: string;
}
