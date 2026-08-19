import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateResourceRequestPayload,
  ResourceRequestItem,
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

  createResourceRequest(
    engagementId: number,
    payload: CreateResourceRequestPayload,
  ): Observable<ResourceRequestItem> {
    const url = RESOURCE_REQUEST_CONSTANTS.API_ENGAGEMENT_RESOURCE_REQUESTS(engagementId);
    return this.http.post<ResourceRequestItem>(url, payload);
  }
}
