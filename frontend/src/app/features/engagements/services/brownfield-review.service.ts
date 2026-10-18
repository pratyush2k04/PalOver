import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import { BrownfieldFindingDto, BrownfieldReadinessResponse } from '../models/brownfield-review/brownfield-review.dtos';
import { CreateBrownfieldFindingRequest, UpdateClientDecisionRequest } from '../models/brownfield-review/brownfield-review.requests';
import { ClientDecision } from '../models/brownfield-review/brownfield-review.enums';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class BrownfieldReviewService {
  private http = inject(HttpClient);

  getFindings(engagementId: number, skipLoader = false): Observable<BrownfieldReadinessResponse> {
    return this.http.get<BrownfieldReadinessResponse>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_FINDINGS(engagementId),
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
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

  deleteFinding(id: number, skipLoader = false): Observable<void> {
    return this.http.delete<void>(
      TECHNICAL_READINESS_API_ENDPOINTS.BROWNFIELD_ITEM(id),
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }
}
