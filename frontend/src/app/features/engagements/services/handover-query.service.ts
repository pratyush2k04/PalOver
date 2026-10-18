import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateHandoverQueryRequest,
  HandoverQueryDto,
  AddHandoverQueryMessageRequest
} from '../models/handover-query.model';
import { HANDOVER_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class HandoverQueryService {
  private http = inject(HttpClient);

  getQueries(engagementId: number): Observable<HandoverQueryDto[]> {
    return this.http.get<HandoverQueryDto[]>(HANDOVER_API_ENDPOINTS.HANDOVER_QUERIES(engagementId), {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }

  raiseQuery(
    engagementId: number,
    request: CreateHandoverQueryRequest,
    skipLoader = false,
  ): Observable<HandoverQueryDto> {
    return this.http.post<HandoverQueryDto>(HANDOVER_API_ENDPOINTS.HANDOVER_QUERIES(engagementId), request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  addMessage(engagementId: number, queryId: number, request: AddHandoverQueryMessageRequest): Observable<HandoverQueryDto> {
    return this.http.post<HandoverQueryDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_QUERY_MESSAGES(engagementId, queryId),
      request
    );
  }

  toggleQueryResolution(engagementId: number, queryId: number): Observable<HandoverQueryDto> {
    return this.http.put<HandoverQueryDto>(
      HANDOVER_API_ENDPOINTS.HANDOVER_QUERY_TOGGLE_RESOLUTION(engagementId, queryId),
      {}
    );
  }

  deleteQuery(engagementId: number, queryId: number): Observable<void> {
    return this.http.delete<void>(
      HANDOVER_API_ENDPOINTS.HANDOVER_QUERY_BY_ID(engagementId, queryId)
    );
  }
}
