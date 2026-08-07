import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangeRequest, CreateChangeRequest, RejectChangeRequest } from '../models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '../../../constants/app.constants';

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
    const url = CHANGE_REQUEST_CONSTANTS.API_ENGAGEMENT_CHANGE_REQUESTS(engagementId);
    return this.http.get<ChangeRequest[]>(url, { params });
  }

  createChangeRequest(engagementId: number, request: CreateChangeRequest): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_ENGAGEMENT_CHANGE_REQUESTS(engagementId);
    return this.http.post<ChangeRequest>(url, request);
  }

  approveChangeRequest(id: number): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_APPROVE_CHANGE_REQUEST(id);
    return this.http.put<ChangeRequest>(url, {});
  }

  rejectChangeRequest(id: number, request: RejectChangeRequest): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_REJECT_CHANGE_REQUEST(id);
    return this.http.put<ChangeRequest>(url, request);
  }
}
