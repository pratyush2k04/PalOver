import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TECHNICAL_READINESS_API_ENDPOINTS } from '@shared/constants/technical-readiness.constants';
import { EnvironmentToolingReadinessResponse, EnvironmentToolingItemDto } from '../models/environment-tooling/environment-tooling.dtos';
import { AddCustomEnvironmentToolingItemRequest, UpdateEnvironmentToolingStatusRequest, UpdateEnvironmentToolingOwnerRequest } from '../models/environment-tooling/environment-tooling.requests';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

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

  deleteItem(id: number, skipLoader = false): Observable<void> {
    return this.http.delete<void>(TECHNICAL_READINESS_API_ENDPOINTS.ENVIRONMENT_TOOLING_ITEM(id), {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }
}
