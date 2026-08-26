import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AllocateResourcesPayload,
  ConfirmAllocationPayload,
  CreateResourceRequestPayload,
  RemoveAllocationPayload,
  ResourceRequestItem,
  ResourcingGapDto,
} from '../models/resource-request.model';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root',
})
export class ResourceRequestService {
  private http = inject(HttpClient);

  getResourceRequests(engagementId: number): Observable<ResourceRequestItem[]> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId);
    return this.http.get<ResourceRequestItem[]>(url);
  }

  getResourcingGap(engagementId: number): Observable<ResourcingGapDto> {
    const url = `${RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId)}/gap`;
    return this.http.get<ResourcingGapDto>(url);
  }

  createResourceRequest(
    engagementId: number,
    payload: CreateResourceRequestPayload,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId);
    return this.http.post<ResourceRequestItem>(url, payload);
  }

  allocateResources(
    engagementId: number,
    requestId: number,
    payload: AllocateResourcesPayload,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_ALLOCATIONS(engagementId, requestId);
    return this.http.post<ResourceRequestItem>(url, payload);
  }

  confirmAllocation(
    engagementId: number,
    requestId: number,
    allocationId: number,
    payload: ConfirmAllocationPayload,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_CONFIRM_ALLOCATION(engagementId, requestId, allocationId);
    return this.http.post<ResourceRequestItem>(url, payload);
  }

  removeAllocation(
    engagementId: number,
    requestId: number,
    allocationId: number,
    payload: RemoveAllocationPayload,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_REMOVE_ALLOCATION(engagementId, requestId, allocationId);
    return this.http.post<ResourceRequestItem>(url, payload);
  }
}
