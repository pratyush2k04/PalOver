import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommunicationCadence,
  CreateCommunicationCadence,
  EngagementParticipant
} from '../models/cadence.model';
import { CADENCE_CONSTANTS } from '../constants/cadence.constants';

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

  addCadence(engagementId: number, request: CreateCommunicationCadence): Observable<CommunicationCadence> {
    const url = CADENCE_CONSTANTS.API_ADD_CADENCE(engagementId);
    return this.http.post<CommunicationCadence>(url, request);
  }
}
