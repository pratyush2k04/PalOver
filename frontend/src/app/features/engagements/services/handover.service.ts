import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommitmentDto,
  CreateCommitmentRequest,
  HandoverInfoDto,
  SaveHandoverDraftRequest,
  UpdateCommitmentRequest
} from '../models/handover.model';

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
