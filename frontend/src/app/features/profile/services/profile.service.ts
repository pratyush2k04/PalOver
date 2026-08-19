import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ME_API_ENDPOINTS } from '@shared/constants/app.constants';
import { ChangePasswordRequest, MeResponse, UpdateMeRequest } from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);

  getMe(): Observable<MeResponse> {
    return this.http.get<MeResponse>(ME_API_ENDPOINTS.GET_ME);
  }

  updateMe(request: UpdateMeRequest): Observable<MeResponse> {
    return this.http.put<MeResponse>(ME_API_ENDPOINTS.UPDATE_ME, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(ME_API_ENDPOINTS.CHANGE_PASSWORD, request);
  }
}
