import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EngagementAllocationItem, HandoverDraftStatus, LeadershipUserItem } from '../models/engagement-allocation.model';
import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class EngagementAllocationService {
  private http = inject(HttpClient);

  getAllEngagements(): Observable<EngagementAllocationItem[]> {
    return this.http.get<EngagementAllocationItem[]>(ENGAGEMENT_API_ENDPOINTS.ALL_ENGAGEMENTS);
  }

  getLeadershipUsers(engagementId: number): Observable<LeadershipUserItem[]> {
    return this.http.get<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_USERS(engagementId));
  }

  getAvailableLeadershipUsers(): Observable<LeadershipUserItem[]> {
    return this.http.get<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_ROLE_USERS);
  }

  getHandoverDraftStatus(engagementId: number): Observable<HandoverDraftStatus> {
    return this.http.get<HandoverDraftStatus>(ENGAGEMENT_API_ENDPOINTS.HANDOVER_DRAFT_STATUS(engagementId));
  }

  assignLeadershipUser(engagementId: number, userId: number): Observable<LeadershipUserItem[]> {
    return this.http.post<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_USERS(engagementId), { userId });
  }

  removeLeadershipUser(engagementId: number, userId: number): Observable<LeadershipUserItem[]> {
    return this.http.delete<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.DELETE_LEADERSHIP_USER(engagementId, userId));
  }
}
