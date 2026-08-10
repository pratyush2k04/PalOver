import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface EngagementInfoDto {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementType: string;
  kickoffDate: string;
  billingStartDate?: string;
  status: string;
  isKickoffReady: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  private http = inject(HttpClient);

  getEngagementInfo(engagementId: number): Observable<EngagementInfoDto> {
    return this.http.get<EngagementInfoDto>(`/api/Engagements/${engagementId}`);
  }
}
