import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LeadershipEngagementListItem } from '../models/portfolio.model';
import { ENGAGEMENT_API_ENDPOINTS } from '@shared/constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private http = inject(HttpClient);

  getPortfolioDashboard(): Observable<LeadershipEngagementListItem[]> {
    return this.http.get<LeadershipEngagementListItem[]>(ENGAGEMENT_API_ENDPOINTS.LEADERSHIP_DASHBOARD);
  }
}
