import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';
import {
  EngagementChecklist,
  UpdateItemStatusRequest,
  UpdateItemApplicabilityRequest,
  ReviewExemptionRequest,
  UpdateItemOwnerRequest,
  BlockChecklistItemRequest,
  SaveChecklistItemEvidenceRequest
} from '../models/engagement-checklist.model';

@Injectable({ providedIn: 'root' })
export class EngagementChecklistService {
  private http = inject(HttpClient);

  getEngagementChecklist(engagementId: number, skipLoader = false): Observable<EngagementChecklist> {
    return this.http.get<EngagementChecklist>(
      ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_CHECKLIST(engagementId),
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
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

  saveItemEvidence(
    engagementId: number,
    itemId: number,
    request: SaveChecklistItemEvidenceRequest,
    skipLoader = false
  ): Observable<EngagementChecklist> {
    return this.http.put<EngagementChecklist>(
      ENGAGEMENT_API_ENDPOINTS.CHECKLIST_ITEM_EVIDENCE(engagementId, itemId),
      request,
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }
}
