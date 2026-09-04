import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangeRequest, CreateChangeRequest, RejectChangeRequest } from '../models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

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

  createChangeRequest(engagementId: number, request: CreateChangeRequest, skipLoader = false): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_ENGAGEMENT_CHANGE_REQUESTS(engagementId);
    return this.http.post<ChangeRequest>(url, request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  approveChangeRequest(id: number): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_APPROVE_CHANGE_REQUEST(id);
    return this.http.put<ChangeRequest>(url, {});
  }

  rejectChangeRequest(id: number, request: RejectChangeRequest, skipLoader = false): Observable<ChangeRequest> {
    const url = CHANGE_REQUEST_CONSTANTS.API_REJECT_CHANGE_REQUEST(id);
    return this.http.put<ChangeRequest>(url, request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }
}
