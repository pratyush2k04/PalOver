import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KPI_REPORT_API_ENDPOINTS } from '@shared/constants/app.constants';
import { KpiReportFilterOptions, KpiReportFilters, KpiReportResponse } from '../models/kpi-report.model';

@Injectable({
  providedIn: 'root'
})
export class KpiReportService {
  private http = inject(HttpClient);

  getFilterOptions(): Observable<KpiReportFilterOptions> {
    return this.http.get<KpiReportFilterOptions>(KPI_REPORT_API_ENDPOINTS.FILTERS);
  }

  getReport(filters: KpiReportFilters): Observable<KpiReportResponse> {
    let params = new HttpParams().set('datePreset', filters.datePreset);

    if (filters.datePreset === 'Custom') {
      if (filters.customFrom) params = params.set('customFrom', filters.customFrom);
      if (filters.customTo) params = params.set('customTo', filters.customTo);
    }

    filters.pmUserIds.forEach((id) => {
      params = params.append('pmUserIds', id);
    });

    filters.engagementTypeIds.forEach((id) => {
      params = params.append('engagementTypeIds', id);
    });

    return this.http.get<KpiReportResponse>(KPI_REPORT_API_ENDPOINTS.REPORT, { params });
  }
}
