import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  HandoverReadinessItemDto,
  AddCustomHandoverReadinessItemRequest,
  UpdateHandoverReadinessStatusRequest
} from '../models/handover-readiness.model';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class HandoverReadinessService {
  private http = inject(HttpClient);

  getItems(engagementId: number, skipLoader = false): Observable<HandoverReadinessItemDto[]> {
    return this.http.get<HandoverReadinessItemDto[]>(
      `/api/engagements/${engagementId}/handover-readiness`,
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }

  addCustomItem(
    engagementId: number,
    request: AddCustomHandoverReadinessItemRequest
  ): Observable<HandoverReadinessItemDto> {
    return this.http.post<HandoverReadinessItemDto>(
      `/api/engagements/${engagementId}/handover-readiness`,
      request
    );
  }

  updateStatus(
    id: number,
    request: UpdateHandoverReadinessStatusRequest
  ): Observable<HandoverReadinessItemDto> {
    return this.http.patch<HandoverReadinessItemDto>(
      `/api/handover-readiness/${id}/status`,
      request
    );
  }

  deleteItem(id: number, skipLoader = false): Observable<void> {
    return this.http.delete<void>(
      `/api/handover-readiness/${id}`,
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }

  saveEvidence(
    engagementId: number,
    itemId: number,
    documentId: number,
    skipLoader = false
  ): Observable<HandoverReadinessItemDto> {
    return this.http.put<HandoverReadinessItemDto>(
      `/api/engagements/${engagementId}/handover-readiness/${itemId}/evidence`,
      { documentId },
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }
}
