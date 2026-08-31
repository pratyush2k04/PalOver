import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardEngagement, PagedResult } from '@shared/models/app-context.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardEngagementsService {
  private http = inject(HttpClient);

  getPage(page: number, pageSize: number): Observable<PagedResult<DashboardEngagement>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<DashboardEngagement>>('/api/home/engagements', { params });
  }
}
