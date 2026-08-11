import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommitmentDto,
  CreateCommitmentRequest,
  CreateRealisticCommitmentRequest,
  HandoverInfoDto,
  RealisticCommitmentDto,
  SaveHandoverDraftRequest,
  UpdateCommitmentRequest,
  UpdateRealisticCommitmentRequest
} from '../models/handover.model';
import { HANDOVER_API_ENDPOINTS } from '../../../shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class HandoverService {
  private http = inject(HttpClient);

  getHandover(engagementId: number): Observable<HandoverInfoDto> {
    return this.http.get<HandoverInfoDto>(HANDOVER_API_ENDPOINTS.HANDOVER(engagementId));
  }

  saveDraft(engagementId: number, request: SaveHandoverDraftRequest): Observable<HandoverInfoDto> {
    return this.http.put<HandoverInfoDto>(HANDOVER_API_ENDPOINTS.HANDOVER_DRAFT(engagementId), request);
  }

  submit(engagementId: number): Observable<HandoverInfoDto> {
    return this.http.post<HandoverInfoDto>(HANDOVER_API_ENDPOINTS.HANDOVER_SUBMIT(engagementId), {});
  }

  addCommitment(engagementId: number, request: CreateCommitmentRequest): Observable<CommitmentDto> {
    return this.http.post<CommitmentDto>(HANDOVER_API_ENDPOINTS.HANDOVER_COMMITMENTS(engagementId), request);
  }

  updateCommitment(engagementId: number, commitmentId: number, request: UpdateCommitmentRequest): Observable<CommitmentDto> {
    return this.http.put<CommitmentDto>(HANDOVER_API_ENDPOINTS.HANDOVER_COMMITMENT_BY_ID(engagementId, commitmentId), request);
  }

  deleteCommitment(engagementId: number, commitmentId: number): Observable<void> {
    return this.http.delete<void>(HANDOVER_API_ENDPOINTS.HANDOVER_COMMITMENT_BY_ID(engagementId, commitmentId));
  }

  createRealisticCommitment(
    engagementId: number,
    commitmentId: number,
    request: CreateRealisticCommitmentRequest
  ): Observable<RealisticCommitmentDto> {
    return this.http.post<RealisticCommitmentDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_REALISTIC_COMMITMENT(engagementId, commitmentId),
      request
    );
  }

  updateRealisticCommitment(
    engagementId: number,
    commitmentId: number,
    request: UpdateRealisticCommitmentRequest
  ): Observable<RealisticCommitmentDto> {
    return this.http.put<RealisticCommitmentDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_REALISTIC_COMMITMENT(engagementId, commitmentId),
      request
    );
  }
}
