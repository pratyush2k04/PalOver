import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AUDIT_LOG_API_ENDPOINTS } from '@shared/constants/app.constants';
import { AuditLogDto, EngagementAuditLogDto, PagedResult, AuditCategory } from '../models/audit-log.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private http = inject(HttpClient);

  getAuditLogPage(
    page: number,
    pageSize: number
  ): Observable<PagedResult<AuditLogDto>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<AuditLogDto>>(AUDIT_LOG_API_ENDPOINTS.LIST, { params });
  }

  getEngagementAuditLogPage(
    engagementId: number,
    page: number,
    pageSize: number,
    userId: number | null = null,
    categories: AuditCategory[] = []
  ): Observable<PagedResult<EngagementAuditLogDto>> {
    let params = new HttpParams()
      .set('engagementId', engagementId)
      .set('page', page)
      .set('pageSize', pageSize);

    if (userId != null) {
      params = params.set('userId', userId);
    }
    
    categories.forEach(cat => {
      params = params.append('categories', cat);
    });

    return this.http.get<PagedResult<EngagementAuditLogDto>>(AUDIT_LOG_API_ENDPOINTS.ENGAGEMENT_LIST, { params });
  }
}
