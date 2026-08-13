import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import {
  BrownfieldFindingDto,
  BrownfieldReadinessResponse,
  CreateBrownfieldFindingRequest,
  UpdateClientDecisionRequest,
  ClientDecision
} from '../models/brownfield-review.model';

@Injectable({
  providedIn: 'root'
})
export class BrownfieldReviewService {
  private http = inject(HttpClient);

  getFindings(engagementId: number): Observable<BrownfieldReadinessResponse> {
    return this.http.get<BrownfieldReadinessResponse>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_FINDINGS(engagementId)
    );
  }

  createFinding(engagementId: number, request: CreateBrownfieldFindingRequest): Observable<BrownfieldFindingDto> {
    return this.http.post<BrownfieldFindingDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_FINDINGS(engagementId),
      request
    );
  }

  updateDecision(id: number, clientDecision: ClientDecision): Observable<BrownfieldFindingDto> {
    const body: UpdateClientDecisionRequest = { clientDecision };
    return this.http.patch<BrownfieldFindingDto>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_DECISION(id),
      body
    );
  }

  deleteFinding(id: number): Observable<void> {
    return this.http.delete<void>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_ITEM(id)
    );
  }
}
