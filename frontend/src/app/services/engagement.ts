import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface MarkSowSignedRequest {
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
}

export interface Engagement {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
  statusId: number;
  checklistVersionId?: number;
}

export interface MarkSowSignedResponse {
  engagement: Engagement;
  warnings: string[];
}

export interface EngagementTypeOption {
  id: number;
  name: string;
}

export interface CommercialModelOption {
  id: number;
  name: string;
}

interface GetEngagementTypesResponse {
  engagementTypes: EngagementTypeOption[];
}

interface GetCommercialModelsResponse {
  commercialModels: CommercialModelOption[];
}

export interface EngagementInfo {
  engagementId: number;
  engagementName: string;
  clientName: string;
  engagementDescription?: string;
  engagementType: string;
  commercialModel: string;
  kickoffDate: string;
  billingStartDate?: string;
  status: string;
}

export interface EngagementOwners {
  projectManagerId?: number;
  projectManagerName?: string;
  techLeadId?: number;
  techLeadName?: string;
}

export interface PmEngagementListItem {
  engagementId: number;
  clientName: string;
  engagementTypeName: string;
  commercialModelName: string;
  kickoffDate: string;
  daysToKickoff: number;
  readinessScore: number | null;
}

export interface PmEngagementListResponse {
  engagements: PmEngagementListItem[];
}

export type EngagementSortBy = 'kickoff' | 'readiness' | 'client';

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  private http = inject(HttpClient);

  markSowSigned(request: MarkSowSignedRequest): Observable<MarkSowSignedResponse> {
    return this.http.post<MarkSowSignedResponse>('/api/Engagements/mark-sow-signed', request);
  }

  getEngagementTypes(): Observable<EngagementTypeOption[]> {
    return this.http
      .get<GetEngagementTypesResponse>('/api/EngagementTypes')
      .pipe(map((response) => response.engagementTypes));
  }

  getCommercialModels(): Observable<CommercialModelOption[]> {
    return this.http
      .get<GetCommercialModelsResponse>('/api/CommercialModels')
      .pipe(map((response) => response.commercialModels));
  }

  getEngagementInfo(id: number): Observable<EngagementInfo> {
    return this.http.get<EngagementInfo>(`/api/Engagements/${id}`);
  }
  //assigning
  getEngagementOwners(id: number): Observable<EngagementOwners> {
    return this.http.get<EngagementOwners>(`/api/Engagements/${id}/owners`);
  }

  assignProjectManager(id: number, userId: number): Observable<EngagementOwners> {
    return this.http.put<EngagementOwners>(`/api/Engagements/${id}/project-manager`, { userId });
  }

  assignTechLead(id: number, userId: number): Observable<EngagementOwners> {
    return this.http.put<EngagementOwners>(`/api/Engagements/${id}/tech-lead`, { userId });
  }
  getMyEngagements(sortBy: EngagementSortBy = 'kickoff'): Observable<PmEngagementListResponse> {
    return this.http.get<PmEngagementListResponse>('/api/Engagements/my-engagements', {
      params: { sortBy }
    });
  }
}
