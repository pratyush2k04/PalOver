import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PROFILE_API_ENDPOINTS } from '@shared/constants/app.constants';
import { ChangePasswordRequest, ProfileResponse, UpdateProfileRequest } from '../models/profile.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(PROFILE_API_ENDPOINTS.GET_PROFILE);
  }

  updateProfile(request: UpdateProfileRequest): Observable<ProfileResponse> {
    return this.http.put<ProfileResponse>(PROFILE_API_ENDPOINTS.UPDATE_PROFILE, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(PROFILE_API_ENDPOINTS.CHANGE_PASSWORD, request);
  }

  getMe(): Observable<ProfileResponse> {
    return this.getProfile();
  }

  updateMe(request: UpdateProfileRequest): Observable<ProfileResponse> {
    return this.updateProfile(request);
  }
}
