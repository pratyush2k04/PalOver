export enum DependencyType {
  Internal = 'Internal',
  External = 'External'
}

export enum DependencyStatus {
  Open = 'Open',
  Met = 'Met',
  AtRisk = 'AtRisk'
}

export interface DependencyDto {
  id: number;
  engagementId: number;
  description: string;
  type: DependencyType;
  ownerId: number;
  ownerName: string;
  ownerUserId?: number | null;
  neededByDate: string; // ISO string
  status: DependencyStatus;
  linkedRiskId?: number | null;
  createdBy?: string | null;
}

export interface CreateDependencyDto {
  description: string;
  type: DependencyType;
  ownerId: number;
  neededByDate: string;
}

export interface UpdateDependencyDto {
  description: string;
  type: DependencyType;
  ownerId: number;
  neededByDate: string;
  status: DependencyStatus;
}
