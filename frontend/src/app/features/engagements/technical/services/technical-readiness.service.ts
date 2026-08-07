import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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
    return this.http.get<AccessItemDto[]>(`/api/engagements/${engagementId}/access-items`);
  }

  addCustomAccessItem(engagementId: number, request: AddCustomAccessItemRequest): Observable<AccessItemDto> {
    return this.http.post<AccessItemDto>(`/api/engagements/${engagementId}/access-items`, request);
  }

  updateAccessItemStatus(id: number, request: UpdateAccessItemStatusRequest): Observable<AccessItemDto> {
    return this.http.patch<AccessItemDto>(`/api/access-items/${id}/status`, request);
  }

  updateAccessItemOwner(id: number, request: UpdateAccessItemOwnerRequest): Observable<AccessItemDto> {
    return this.http.patch<AccessItemDto>(`/api/access-items/${id}/owner`, request);
  }
}
