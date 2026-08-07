import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RealisticCommitmentDto {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string | null;
  isPossible: boolean;
}

export interface CommitmentDto {
  commitmentId: number;
  handoverId: number;
  description: string;
  commitmentTypeId: number;
  title: string;
  timeline: number;
  timelineUnit: string;
  realisticCommitment?: RealisticCommitmentDto | null;
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
  description: string;
  commitmentTypeId: number;
  title: string;
  timeline: number;
  timelineUnit: string;
}

export interface UpdateCommitmentRequest {
  description: string;
  commitmentTypeId: number;
  title: string;
  timeline: number;
  timelineUnit: string;
}

export interface CreateRealisticCommitmentRequest {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string | null;
  isPossible: boolean;
}

export interface UpdateRealisticCommitmentRequest {
  description: string;
  timeline: number;
  timelineUnit: string;
  acknowledgementMessage?: string | null;
  isPossible: boolean;
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

  createRealisticCommitment(engagementId: number, commitmentId: number, request: CreateRealisticCommitmentRequest): Observable<RealisticCommitmentDto> {
    return this.http.post<RealisticCommitmentDto>(`${this.baseUrl}/${engagementId}/handover/commitments/${commitmentId}/realistic`, request);
  }

  updateRealisticCommitment(engagementId: number, commitmentId: number, request: UpdateRealisticCommitmentRequest): Observable<RealisticCommitmentDto> {
    return this.http.put<RealisticCommitmentDto>(`${this.baseUrl}/${engagementId}/handover/commitments/${commitmentId}/realistic`, request);
  }
}
