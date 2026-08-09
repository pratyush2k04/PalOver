import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from 'src/app/constants/technical-readiness.constants';
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
    return this.http.get<EnvironmentToolingReadinessResponse>(TECHNICAL_READINESS_API_ENDPOINTS.ENVIRONMENT_TOOLING_ITEMS(engagementId));
  }

  addCustomItem(engagementId: number, request: AddCustomEnvironmentToolingItemRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.post<EnvironmentToolingItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ENVIRONMENT_TOOLING_ITEMS(engagementId), request);
  }

  updateStatus(id: number, request: UpdateEnvironmentToolingStatusRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.patch<EnvironmentToolingItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ENVIRONMENT_TOOLING_STATUS(id), request);
  }

  updateOwner(id: number, request: UpdateEnvironmentToolingOwnerRequest): Observable<EnvironmentToolingItemDto> {
    return this.http.patch<EnvironmentToolingItemDto>(TECHNICAL_READINESS_API_ENDPOINTS.ENVIRONMENT_TOOLING_OWNER(id), request);
  }
}
