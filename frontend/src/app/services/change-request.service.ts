import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangeRequest, CreateChangeRequest, RejectChangeRequest } from '../models/change-request.model';

@Injectable({
  providedIn: 'root'
})
export class ChangeRequestService {
  private http = inject(HttpClient);

  getChangeRequests(engagementId: number, status?: string): Observable<ChangeRequest[]> {
    let params = new HttpParams();
    if (status && status !== 'All') {
      params = params.set('status', status);
    }
    return this.http.get<ChangeRequest[]>(`/api/engagements/${engagementId}/change-requests`, { params });
  }

  createChangeRequest(engagementId: number, request: CreateChangeRequest): Observable<ChangeRequest> {
    return this.http.post<ChangeRequest>(`/api/engagements/${engagementId}/change-requests`, request);
  }

  approveChangeRequest(id: number): Observable<ChangeRequest> {
    return this.http.put<ChangeRequest>(`/api/change-requests/${id}/approve`, {});
  }

  rejectChangeRequest(id: number, request: RejectChangeRequest): Observable<ChangeRequest> {
    return this.http.put<ChangeRequest>(`/api/change-requests/${id}/reject`, request);
  }
}
