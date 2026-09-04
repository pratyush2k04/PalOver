import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  RiskComment,
  CreateRiskCommentRequest,
  UpdateRiskCommentRequest,
  EngagementMember
} from '../models/risk.model';
import { RISK_COMMENT_API_ENDPOINTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class RiskCommentService {
  private http = inject(HttpClient);

  getComments(engagementId: number, riskId: number): Observable<RiskComment[]> {
    return this.http.get<RiskComment[]>(RISK_COMMENT_API_ENDPOINTS.GET_COMMENTS(engagementId, riskId), {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }

  addComment(engagementId: number, riskId: number, request: CreateRiskCommentRequest): Observable<RiskComment> {
    return this.http.post<RiskComment>(RISK_COMMENT_API_ENDPOINTS.ADD_COMMENT(engagementId, riskId), request);
  }

  updateComment(engagementId: number, riskId: number, commentId: number, request: UpdateRiskCommentRequest): Observable<RiskComment> {
    return this.http.put<RiskComment>(RISK_COMMENT_API_ENDPOINTS.UPDATE_COMMENT(engagementId, riskId, commentId), request);
  }

  getEngagementMembers(engagementId: number): Observable<EngagementMember[]> {
    return this.http.get<EngagementMember[]>(RISK_COMMENT_API_ENDPOINTS.GET_MEMBERS(engagementId), {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }
}
