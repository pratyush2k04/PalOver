import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EngagementChecklist } from '../models/engagement-checklist.model';

@Injectable({ providedIn: 'root' })
export class EngagementChecklistService {
  private http = inject(HttpClient);

  getEngagementChecklist(engagementId: number): Observable<EngagementChecklist> {
    return this.http.get<EngagementChecklist>(
      `/api/engagements/${engagementId}/workspace/checklist`
    );
  }
}
