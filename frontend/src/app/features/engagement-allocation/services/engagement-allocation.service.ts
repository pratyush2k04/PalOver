import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EngagementAllocationItem, HandoverDraftStatus, LeadershipUserItem } from '../models/engagement-allocation.model';
import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class EngagementAllocationService {
  private http = inject(HttpClient);

  getAllEngagements(): Observable<EngagementAllocationItem[]> {
    return this.http.get<EngagementAllocationItem[]>(ENGAGEMENT_API_ENDPOINTS.ALL_ENGAGEMENTS);
  }

  getLeadershipUsers(engagementId: number, skipLoader = false): Observable<LeadershipUserItem[]> {
    return this.http.get<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_USERS(engagementId), {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  getAvailableLeadershipUsers(): Observable<LeadershipUserItem[]> {
    return this.http.get<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_ROLE_USERS);
  }

  getHandoverDraftStatus(engagementId: number, skipLoader = false): Observable<HandoverDraftStatus> {
    return this.http.get<HandoverDraftStatus>(ENGAGEMENT_API_ENDPOINTS.HANDOVER_DRAFT_STATUS(engagementId), {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  updateLeadershipUsers(engagementId: number, userIds: number[], skipLoader = false): Observable<LeadershipUserItem[]> {
    return this.http.put<LeadershipUserItem[]>(ENGAGEMENT_API_ENDPOINTS.UPDATE_LEADERSHIP_USERS(engagementId), { userIds }, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }
}
