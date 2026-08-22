import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  CommercialModelOption,
  EngagementInfo,
  EngagementOwners,
  EngagementTypeOption,
  GetCommercialModelsResponse,
  GetEngagementTypesResponse,
  MarkSowSignedRequest,
  MarkSowSignedResponse,
  EngagementListResponse,
  EngagementSortBy,
  DayOneSignOffRequest
} from '../models/engagement.model';
import { EvidenceSnapshot } from '../models/evidence-snapshot.model';
import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';

export interface EngagementOpenItemsSummary {
  checklistOpenCount: number;
  accessOpenCount: number;
  openRisksCount: number;
  resourcingOpenCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  private http = inject(HttpClient);

  markSowSigned(request: MarkSowSignedRequest): Observable<MarkSowSignedResponse> {
    return this.http.post<MarkSowSignedResponse>(ENGAGEMENT_API_ENDPOINTS.MARK_SOW_SIGNED, request);
  }

  getEngagementTypes(): Observable<EngagementTypeOption[]> {
    return this.http
      .get<GetEngagementTypesResponse>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_TYPES)
      .pipe(map((response) => response.engagementTypes));
  }

  getCommercialModels(): Observable<CommercialModelOption[]> {
    return this.http
      .get<GetCommercialModelsResponse>(ENGAGEMENT_API_ENDPOINTS.COMMERCIAL_MODELS)
      .pipe(map((response) => response.commercialModels));
  }

  getEngagementInfo(id: number): Observable<EngagementInfo> {
    return this.http.get<EngagementInfo>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_INFO(id));
  }

  getEngagementOwners(id: number): Observable<EngagementOwners> {
    return this.http.get<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_OWNERS(id));
  }

  getEngagementUsers(id: number): Observable<any[]> {
    return this.http.get<any[]>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_USERS(id));
  }

  assignProjectManager(id: number, userId: number): Observable<EngagementOwners> {
    return this.http.put<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ASSIGN_PROJECT_MANAGER(id), { userId });
  }

  assignTechLead(id: number, userId: number): Observable<EngagementOwners> {
    return this.http.put<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ASSIGN_TECH_LEAD(id), { userId });
  }

  getMyEngagements(sortBy: EngagementSortBy = 'kickoff'): Observable<EngagementListResponse> {
    return this.http.get<EngagementListResponse>(ENGAGEMENT_API_ENDPOINTS.MY_ENGAGEMENTS, {
      params: { sortBy }
    });
  }

  getOpenItemsSummary(id: number): Observable<EngagementOpenItemsSummary> {
    return this.http.get<EngagementOpenItemsSummary>(ENGAGEMENT_API_ENDPOINTS.OPEN_ITEMS_SUMMARY(id));
  }

  signOffDayOne(id: number, request: DayOneSignOffRequest): Observable<EngagementInfo> {
    return this.http.post<EngagementInfo>(ENGAGEMENT_API_ENDPOINTS.DAY_ONE_SIGN_OFF(id), request);
  }

  getEvidenceSnapshot(id: number): Observable<EvidenceSnapshot> {
    return this.http.get<EvidenceSnapshot>(ENGAGEMENT_API_ENDPOINTS.EVIDENCE_SNAPSHOT(id));
  }
}
