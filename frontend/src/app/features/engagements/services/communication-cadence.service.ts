import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommunicationCadence,
  CreateCommunicationCadence,
  EngagementParticipant
} from '../models/cadence.model';
import { CADENCE_CONSTANTS } from '@shared/constants/app.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class CommunicationCadenceService {
  private http = inject(HttpClient);

  getCadences(engagementId: number): Observable<CommunicationCadence[]> {
    const url = CADENCE_CONSTANTS.API_GET_CADENCES(engagementId);
    return this.http.get<CommunicationCadence[]>(url);
  }

  getAssignedParticipants(engagementId: number): Observable<EngagementParticipant[]> {
    const url = CADENCE_CONSTANTS.API_GET_ASSIGNED_PARTICIPANTS(engagementId);
    return this.http.get<EngagementParticipant[]>(url);
  }

  addCadence(
    engagementId: number,
    request: CreateCommunicationCadence,
    skipLoader = false
  ): Observable<CommunicationCadence> {
    const url = CADENCE_CONSTANTS.API_ADD_CADENCE(engagementId);
    return this.http.post<CommunicationCadence>(url, request, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }
}
