import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateHandoverQueryRequest,
  HandoverQueryDto,
  RespondHandoverQueryRequest
} from '../models/handover-query.model';
import { HANDOVER_API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class HandoverQueryService {
  private http = inject(HttpClient);

  getQueries(engagementId: number): Observable<HandoverQueryDto[]> {
    return this.http.get<HandoverQueryDto[]>(HANDOVER_API_ENDPOINTS.HANDOVER_QUERIES(engagementId));
  }

  raiseQuery(engagementId: number, request: CreateHandoverQueryRequest): Observable<HandoverQueryDto> {
    return this.http.post<HandoverQueryDto>(HANDOVER_API_ENDPOINTS.HANDOVER_QUERIES(engagementId), request);
  }

  respondToQuery(engagementId: number, queryId: number, request: RespondHandoverQueryRequest): Observable<HandoverQueryDto> {
    return this.http.put<HandoverQueryDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_QUERY_RESPOND(engagementId, queryId),
      request
    );
  }

  resolveQuery(engagementId: number, queryId: number, isResolved: boolean): Observable<HandoverQueryDto> {
    return this.http.put<HandoverQueryDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_QUERY_RESOLVE(engagementId, queryId),
      { isResolved }
    );
  }
}
