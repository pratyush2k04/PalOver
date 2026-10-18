import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AllocateResourcesPayload,
  CreateResourceRequestPayload,
  RemoveAllocationPayload,
  ResourceRequestItem,
} from '../models/resource-request.model';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root',
})
export class ResourceRequestService {
  private http = inject(HttpClient);

  getResourceRequests(engagementId: number): Observable<ResourceRequestItem[]> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId);
    return this.http.get<ResourceRequestItem[]>(url);
  }

  createResourceRequest(
    engagementId: number,
    payload: CreateResourceRequestPayload,
    skipLoader = false,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId);
    return this.http.post<ResourceRequestItem>(url, payload, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  allocateResources(
    engagementId: number,
    requestId: number,
    payload: AllocateResourcesPayload,
    skipLoader = false,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_ALLOCATIONS(engagementId, requestId);
    return this.http.post<ResourceRequestItem>(url, payload, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  removeAllocation(
    engagementId: number,
    requestId: number,
    allocationId: number,
    payload: RemoveAllocationPayload,
    skipLoader = false,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_REMOVE_ALLOCATION(engagementId, requestId, allocationId);
    return this.http.post<ResourceRequestItem>(url, payload, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }
}
