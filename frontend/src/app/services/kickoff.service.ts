import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KickoffRecord, KickoffActionItem, KickoffActionItemStatus } from '../models/kickoff.model';

@Injectable({
  providedIn: 'root'
})
export class KickoffService {
  private http = inject(HttpClient);

  getKickoffRecord(engagementId: number): Observable<KickoffRecord> {
    return this.http.get<KickoffRecord>(`/api/engagements/${engagementId}/kickoff`);
  }

  saveKickoffRecord(engagementId: number, outcomes: string): Observable<KickoffRecord> {
    return this.http.post<KickoffRecord>(`/api/engagements/${engagementId}/kickoff`, { outcomes });
  }

  getOpenActionItems(engagementId: number): Observable<KickoffActionItem[]> {
    return this.http.get<KickoffActionItem[]>(`/api/engagements/${engagementId}/kickoff/actions/open`);
  }

  createActionItem(engagementId: number, dto: Partial<KickoffActionItem>): Observable<KickoffActionItem> {
    return this.http.post<KickoffActionItem>(`/api/engagements/${engagementId}/kickoff/actions`, dto);
  }

  updateActionItemStatus(engagementId: number, actionItemId: number, status: KickoffActionItemStatus): Observable<KickoffActionItem> {
    return this.http.patch<KickoffActionItem>(`/api/engagements/${engagementId}/kickoff/actions/${actionItemId}/status`, { status });
  }

  updateActionItem(engagementId: number, actionItemId: number, dto: Partial<KickoffActionItem>): Observable<KickoffActionItem> {
    return this.http.put<KickoffActionItem>(`/api/engagements/${engagementId}/kickoff/actions/${actionItemId}`, dto);
  }

  deleteActionItem(engagementId: number, actionItemId: number): Observable<void> {
    return this.http.delete<void>(`/api/engagements/${engagementId}/kickoff/actions/${actionItemId}`);
  }
}
