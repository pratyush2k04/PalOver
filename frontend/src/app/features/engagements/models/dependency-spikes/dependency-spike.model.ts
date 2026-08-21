export enum DependencySpikeStatus {
  InProgress = 1,
  Passed = 2,
  Failed = 3
}

export interface DependencySpikeDto {
  id: number;
  engagementId: number;
  dependencyName: string;
  objective: string;
  status: DependencySpikeStatus | string;
  ownerId?: number | null;
  ownerFullName?: string | null;
  outcomeFindings?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateDependencySpikeRequest {
  dependencyName: string;
  objective: string;
  status: DependencySpikeStatus;
  ownerId?: number | null;
  outcomeFindings?: string | null;
}

export interface UpdateDependencySpikeRequest {
  dependencyName: string;
  objective: string;
  status: DependencySpikeStatus;
  ownerId?: number | null;
  outcomeFindings?: string | null;
}
