export interface CommitmentDto {
  commitmentId: number;
  handoverId: number;
  description: string;
  commitmentTypeId: number;
  quantity?: string;
}

export interface HandoverInfoDto {
  handoverId: number;
  engagementId: number;
  status: string;
  clientBackground?: string;
  businessProblem: string;
  clientSentiment: string;
  dealOrigin?: string;
  hasNoCommitments: boolean;
  commitments: CommitmentDto[];
}

export interface SaveHandoverDraftRequest {
  clientBackground?: string;
  businessProblem?: string;
  clientSentiment?: string;
  dealOrigin?: string;
  hasNoCommitments: boolean;
}

export interface CreateCommitmentRequest {
  description: string;
  commitmentTypeId: number;
  quantity?: string;
}

export interface UpdateCommitmentRequest {
  description: string;
  commitmentTypeId: number;
  quantity?: string;
}
