import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Risk,
  CreateRiskRequest,
  UpdateRiskRequest,
  CloseRiskRequest,
  RiskStatus
} from '../models/risk.model';
import { RISK_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class RiskService {
  private http = inject(HttpClient);

  getRisks(engagementId: number, status?: RiskStatus, ownerId?: number): Observable<Risk[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    if (ownerId) {
      params = params.set('ownerId', ownerId.toString());
    }

    return this.http.get<Risk[]>(RISK_API_ENDPOINTS.GET_RISKS(engagementId), { params });
  }

  createRisk(engagementId: number, request: CreateRiskRequest): Observable<Risk> {
    return this.http.post<Risk>(RISK_API_ENDPOINTS.CREATE_RISK(engagementId), request);
  }

  updateRisk(engagementId: number, riskId: number, request: UpdateRiskRequest): Observable<Risk> {
    return this.http.put<Risk>(RISK_API_ENDPOINTS.UPDATE_RISK(engagementId, riskId), request);
  }

  closeRisk(engagementId: number, riskId: number, request: CloseRiskRequest, skipLoader = false): Observable<Risk> {
    return this.http.post<Risk>(RISK_API_ENDPOINTS.CLOSE_RISK(engagementId, riskId), request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  reopenRisk(engagementId: number, riskId: number): Observable<Risk> {
    return this.http.post<Risk>(RISK_API_ENDPOINTS.REOPEN_RISK(engagementId, riskId), {});
  }
}
