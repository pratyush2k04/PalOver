import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
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
  MyEngagementsPage,
  DayOneSignOffRequest
} from '../models/engagement.model';
import { EvidenceSnapshot } from '../models/evidence-snapshot.model';
import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

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

  private currentEngagementState = signal<EngagementInfo | null>(null);
  readonly currentEngagement = this.currentEngagementState.asReadonly();
  readonly isSignedOff = computed(() => !!this.currentEngagementState()?.dayOneSignOff);

  private ownersState = signal<EngagementOwners | null>(null);
  readonly owners = this.ownersState.asReadonly();

  setCurrentEngagement(info: EngagementInfo | null): void {
    this.currentEngagementState.set(info);
  }

  markSowSigned(request: MarkSowSignedRequest, skipLoader = false): Observable<MarkSowSignedResponse> {
    return this.http.post<MarkSowSignedResponse>(ENGAGEMENT_API_ENDPOINTS.MARK_SOW_SIGNED, request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
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
    return this.http
      .get<EngagementInfo>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_INFO(id))
      .pipe(tap((info) => this.currentEngagementState.set(info)));
  }

  getEngagementOwners(id: number, skipLoader = false): Observable<EngagementOwners> {
    return this.http
      .get<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_OWNERS(id), {
        context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
      })
      .pipe(tap((owners) => this.ownersState.set(owners)));
  }

  assignProjectManager(id: number, userId: number, skipLoader = false): Observable<EngagementOwners> {
    return this.http
      .put<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ASSIGN_PROJECT_MANAGER(id), { userId }, {
        context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
      })
      .pipe(tap((owners) => this.ownersState.set(owners)));
  }

  assignTechLead(id: number, userId: number, skipLoader = false): Observable<EngagementOwners> {
    return this.http
      .put<EngagementOwners>(ENGAGEMENT_API_ENDPOINTS.ASSIGN_TECH_LEAD(id), { userId }, {
        context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
      })
      .pipe(tap((owners) => this.ownersState.set(owners)));
  }

  getMyEngagements(sortBy: EngagementSortBy = 'kickoff'): Observable<EngagementListResponse> {
    return this.http.get<EngagementListResponse>(ENGAGEMENT_API_ENDPOINTS.MY_ENGAGEMENTS, {
      params: { sortBy }
    });
  }

  getMyEngagementsPage(status: 'Active' | 'Past', sortBy: EngagementSortBy, page: number, pageSize: number): Observable<MyEngagementsPage> {
    const params = new HttpParams()
      .set('status', status)
      .set('sortBy', sortBy)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<MyEngagementsPage>(ENGAGEMENT_API_ENDPOINTS.MY_ENGAGEMENTS_PAGE, { params });
  }

  getOpenItemsSummary(id: number): Observable<EngagementOpenItemsSummary> {
    return this.http.get<EngagementOpenItemsSummary>(ENGAGEMENT_API_ENDPOINTS.OPEN_ITEMS_SUMMARY(id));
  }

  signOffDayOne(id: number, request: DayOneSignOffRequest, skipLoader = false): Observable<EngagementInfo> {
    return this.http
      .post<EngagementInfo>(ENGAGEMENT_API_ENDPOINTS.DAY_ONE_SIGN_OFF(id), request, {
        context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
      })
      .pipe(tap((info) => this.currentEngagementState.set(info)));
  }

  getEvidenceSnapshot(id: number): Observable<EvidenceSnapshot> {
    return this.http.get<EvidenceSnapshot>(ENGAGEMENT_API_ENDPOINTS.EVIDENCE_SNAPSHOT(id));
  }
}
