import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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

@Injectable({
  providedIn: 'root'
})
export class HandoverService {
  private http = inject(HttpClient);
  private baseUrl = '/api/engagements';

  getHandover(engagementId: number): Observable<HandoverInfoDto> {
    return this.http.get<HandoverInfoDto>(`${this.baseUrl}/${engagementId}/handover`);
  }

  saveDraft(engagementId: number, request: SaveHandoverDraftRequest): Observable<HandoverInfoDto> {
    return this.http.put<HandoverInfoDto>(`${this.baseUrl}/${engagementId}/handover/draft`, request);
  }

  submit(engagementId: number): Observable<HandoverInfoDto> {
    return this.http.post<HandoverInfoDto>(`${this.baseUrl}/${engagementId}/handover/submit`, {});
  }

  addCommitment(engagementId: number, request: CreateCommitmentRequest): Observable<CommitmentDto> {
    return this.http.post<CommitmentDto>(`${this.baseUrl}/${engagementId}/handover/commitments`, request);
  }

  updateCommitment(engagementId: number, commitmentId: number, request: UpdateCommitmentRequest): Observable<CommitmentDto> {
    return this.http.put<CommitmentDto>(`${this.baseUrl}/${engagementId}/handover/commitments/${commitmentId}`, request);
  }

  deleteCommitment(engagementId: number, commitmentId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${engagementId}/handover/commitments/${commitmentId}`);
  }
}
