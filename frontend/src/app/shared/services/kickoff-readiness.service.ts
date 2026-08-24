import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface KickoffReadinessDto {
  score: number;
  totalApplicable: number;
  totalPassed: number;
  handoverFieldsApplicable: number;
  handoverFieldsPassed: number;
  commitmentsApplicable: number;
  commitmentsPassed: number;
  accessItemsApplicable: number;
  accessItemsPassed: number;
  checklistItemsApplicable: number;
  checklistItemsPassed: number;
}

@Injectable({
  providedIn: 'root'
})
export class KickoffReadinessService {
  private http = inject(HttpClient);

  getReadiness(engagementId: number): Observable<KickoffReadinessDto> {
    return this.http.get<KickoffReadinessDto>(`/api/Engagements/${engagementId}/kickoff-readiness`);
  }

  markKickoffReady(engagementId: number): Observable<void> {
    return this.http.post<void>(`/api/Engagements/${engagementId}/kickoff-ready`, {});
  }

  revertKickoffReady(engagementId: number): Observable<void> {
    return this.http.delete<void>(`/api/Engagements/${engagementId}/kickoff-ready`);
  }
}
