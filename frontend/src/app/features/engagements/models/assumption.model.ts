export enum AssumptionStatus {
  Unvalidated = 1,
  Validated = 2,
  Invalid = 3
}

export interface AssumptionDto {
  id: number;
  engagementId: number;
  description: string;
  rationale: string;
  ownerId: number;
  ownerName: string;
  status: AssumptionStatus;
  linkedRiskId?: number | null;
}

export interface CreateAssumptionDto {
  description: string;
  rationale: string;
}

export interface UpdateAssumptionDto {
  description: string;
  rationale: string;
}

export interface ValidateAssumptionDto {
  status: AssumptionStatus;
}
