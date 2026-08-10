import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from 'src/app/constants/technical-readiness.constants';
import {
  AccessItemDto,
  AddCustomAccessItemRequest,
  UpdateAccessItemStatusRequest,
  UpdateAccessItemOwnerRequest
} from '../models/technical-readiness.model';

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
}
