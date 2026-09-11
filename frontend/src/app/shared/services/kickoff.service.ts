import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KickoffRecord, KickoffActionItem, KickoffActionItemStatus } from '@shared/models/kickoff.model';
import { KICKOFF_API_ENDPOINTS } from '@shared/constants/kickoff.constants';

@Injectable({
  providedIn: 'root'
})
export class KickoffService {
  private http = inject(HttpClient);

  getKickoffRecord(engagementId: number): Observable<KickoffRecord> {
    return this.http.get<KickoffRecord>(KICKOFF_API_ENDPOINTS.RECORD(engagementId));
  }

  saveKickoffRecord(engagementId: number, outcomes: string): Observable<KickoffRecord> {
    return this.http.post<KickoffRecord>(KICKOFF_API_ENDPOINTS.RECORD(engagementId), { outcomes });
  }

  createActionItem(engagementId: number, dto: Partial<KickoffActionItem>): Observable<KickoffActionItem> {
    return this.http.post<KickoffActionItem>(KICKOFF_API_ENDPOINTS.ACTIONS_CREATE(engagementId), dto);
  }

  updateActionItemStatus(engagementId: number, actionItemId: number, status: KickoffActionItemStatus): Observable<KickoffActionItem> {
    return this.http.patch<KickoffActionItem>(KICKOFF_API_ENDPOINTS.ACTIONS_UPDATE_STATUS(engagementId, actionItemId), { status });
  }

  deleteActionItem(engagementId: number, actionItemId: number): Observable<void> {
    return this.http.delete<void>(KICKOFF_API_ENDPOINTS.ACTIONS_DELETE(engagementId, actionItemId));
  }
}
