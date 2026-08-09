import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  EnvironmentToolingItemDto,
  EnvironmentToolingReadinessResponse,
  AddCustomEnvironmentToolingItemRequest,
  UpdateEnvironmentToolingStatusRequest,
  UpdateEnvironmentToolingOwnerRequest
} from '../models/environment-tooling.model';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentToolingService {
  private http = inject(HttpClient);

  getItems(engagementId: number): Observable<EnvironmentToolingReadinessResponse> {
    return this.http.get<EnvironmentToolingReadinessResponse>(`/api/engagements/${engagementId}/environment-tooling`);
  }

  addCustomItem(engagementId: number, request: AddCustomEnvironmentToolingItemRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.post<EnvironmentToolingItemDto>(`/api/engagements/${engagementId}/environment-tooling`, request);
  }

  updateStatus(id: number, request: UpdateEnvironmentToolingStatusRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.patch<EnvironmentToolingItemDto>(`/api/environment-tooling/${id}/status`, request);
  }

  updateOwner(id: number, request: UpdateEnvironmentToolingOwnerRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.patch<EnvironmentToolingItemDto>(`/api/environment-tooling/${id}/owner`, request);
  }
}
