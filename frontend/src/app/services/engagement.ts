import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface MarkSowSignedRequest {
  clientName?: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
}

export interface Engagement {
  engagementId: number;
  clientName: string;
  engagementDescription?: string;
  engagementTypeId: number;
  commercialModelId: number;
  kickoffDate: string;
  billingStartDate?: string;
  statusId: number;
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
}
