import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  HandoverReadinessItemDto,
  AddCustomHandoverReadinessItemRequest,
  UpdateHandoverReadinessStatusRequest
} from '../models/handover-readiness.model';

@Injectable({
  providedIn: 'root'
})
export class HandoverReadinessService {
  private http = inject(HttpClient);

  getItems(engagementId: number): Observable<HandoverReadinessItemDto[]> {
    return this.http.get<HandoverReadinessItemDto[]>(
      `/api/engagements/${engagementId}/handover-readiness`
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

  deleteItem(id: number): Observable<void> {
    return this.http.delete<void>(
      `/api/handover-readiness/${id}`
    );
  }
}
