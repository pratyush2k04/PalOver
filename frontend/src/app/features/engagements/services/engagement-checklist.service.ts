import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ENGAGEMENT_API_ENDPOINTS } from '../../../constants/app.constants';
import {
  EngagementChecklist,
  UpdateItemStatusRequest,
  UpdateItemApplicabilityRequest,
  ReviewExemptionRequest,
  UpdateItemOwnerRequest,
  BlockChecklistItemRequest
} from '../models/engagement-checklist.model';

@Injectable({ providedIn: 'root' })
export class EngagementChecklistService {
  private http = inject(HttpClient);

  getEngagementChecklist(engagementId: number): Observable<EngagementChecklist> {
    return this.http.get<EngagementChecklist>(
      ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_CHECKLIST(engagementId)
    );
  }

  updateItemStatus(
    engagementId: number,
    itemId: number,
    request: UpdateItemStatusRequest
  ): Observable<EngagementChecklist> {
    return this.http.patch<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/status`,
      request
    );
  }

  updateItemApplicability(
    engagementId: number,
    itemId: number,
    request: UpdateItemApplicabilityRequest
  ): Observable<EngagementChecklist> {
    return this.http.post<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/applicability`,
      request
    );
  }

  reviewExemption(
    engagementId: number,
    itemId: number,
    request: ReviewExemptionRequest
  ): Observable<EngagementChecklist> {
    return this.http.post<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/exemption/review`,
      request
    );
  }

  updateItemOwner(
    engagementId: number,
    itemId: number,
    request: UpdateItemOwnerRequest
  ): Observable<EngagementChecklist> {
    return this.http.patch<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/owner`,
      request
    );
  }

  blockItem(
    engagementId: number,
    itemId: number,
    request: BlockChecklistItemRequest
  ): Observable<EngagementChecklist> {
    return this.http.post<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/block`,
      request
    );
  }

  unblockItem(engagementId: number, itemId: number): Observable<EngagementChecklist> {
    return this.http.post<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist/items/${itemId}/unblock`,
      {}
    );
  }
}
