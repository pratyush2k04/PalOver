import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import { AccessItemDto } from '../models/access-tracker/access-tracker.dtos';
import { AddCustomAccessItemRequest, UpdateAccessItemStatusRequest, UpdateAccessItemOwnerRequest } from '../models/access-tracker/access-tracker.requests';
import { DayOneReadinessSummaryDto } from '../models/technical-readiness/technical-summary.dtos';

@Injectable({
  providedIn: 'root'
})
export class TechnicalReadinessService {
  private http = inject(HttpClient);

  getAccessItems(engagementId: number): Observable<AccessItemDto[]> {
    return this.http.get<AccessItemDto[]>(TECHNICAL_READINESS_API_ENDPOINTS.ACCESS_ITEMS(engagementId));
  }

  addCustomAccessItem(engagementId: number, request: AddCustomAccessItemRequest): Observable<AccessItemDto> {
    return this.http.post<AccessItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ACCESS_ITEMS(engagementId), request);
  }

  updateAccessItemStatus(id: number, request: UpdateAccessItemStatusRequest): Observable<AccessItemDto> {
    return this.http.patch<AccessItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ACCESS_ITEM_STATUS(id), request);
  }

  updateAccessItemOwner(id: number, request: UpdateAccessItemOwnerRequest): Observable<AccessItemDto> {
    return this.http.patch<AccessItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ACCESS_ITEM_OWNER(id), request);
  }

  getTechnicalSummary(engagementId: number): Observable<DayOneReadinessSummaryDto> {
    return this.http.get<DayOneReadinessSummaryDto>(TECHNICAL_READINESS_API_ENDPOINTS.TECHNICAL_SUMMARY(engagementId));
  }
}
