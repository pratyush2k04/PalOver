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
  currentEscalationLevel?: string;
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
