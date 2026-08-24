export interface RealisticCommitmentDto {
  id: number;
  commitmentId: number;
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string;
}

export interface CreateRealisticCommitmentRequest {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string | null;
}

export interface UpdateRealisticCommitmentRequest {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string | null;
}

export interface CommitmentDto {
  commitmentId: number;
  handoverId: number;
  title: string;
  description: string;
  commitmentTypeId: number;
  timeline: number;
  timelineUnit: string;
  realisticCommitment?: RealisticCommitmentDto;
  isVarianceFlagged?: boolean;
  varianceDescription?: string;
}

export interface HandoverInfoDto {
  handoverId: number;
  engagementId: number;
  status: string;
  clientBackground?: string;
  businessProblem: string;
  clientSentiment: string;
  dealOrigin?: string;
  commitments: CommitmentDto[];
}

export interface SaveHandoverDraftRequest {
  clientBackground?: string;
  businessProblem?: string;
  clientSentiment?: string;
  dealOrigin?: string;
}

export interface CreateCommitmentRequest {
  title: string;
  description: string;
  commitmentTypeId: number;
  timeline: number;
  timelineUnit: string;
}

export interface UpdateCommitmentRequest {
  title: string;
  description: string;
  commitmentTypeId: number;
  timeline: number;
  timelineUnit: string;
}
