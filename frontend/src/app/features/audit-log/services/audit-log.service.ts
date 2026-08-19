import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AUDIT_LOG_API_ENDPOINTS } from '@shared/constants/app.constants';
import { AuditLogDto, CursorPagedResult } from '../models/audit-log.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private http = inject(HttpClient);

  getAuditLogPage(
    cursor: number | null,
    limit: number,
    engagementId?: number | null
  ): Observable<CursorPagedResult<AuditLogDto>> {
    let params = new HttpParams().set('limit', limit);
    if (cursor != null) {
      params = params.set('cursor', cursor);
    }
    if (engagementId != null) {
      params = params.set('engagementId', engagementId);
    }
    return this.http.get<CursorPagedResult<AuditLogDto>>(AUDIT_LOG_API_ENDPOINTS.LIST, { params });
  }
}
