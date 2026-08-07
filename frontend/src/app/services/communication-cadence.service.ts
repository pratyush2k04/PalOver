import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommunicationCadence,
  CreateCommunicationCadence,
  EngagementParticipant
} from '../models/cadence.model';

@Injectable({
  providedIn: 'root'
})
export class CommunicationCadenceService {
  private http = inject(HttpClient);

  getCadences(engagementId: number): Observable<CommunicationCadence[]> {
    return this.http.get<CommunicationCadence[]>(`/api/engagements/${engagementId}/cadences`);
  }

  getAssignedParticipants(engagementId: number): Observable<EngagementParticipant[]> {
    return this.http.get<EngagementParticipant[]>(`/api/engagements/${engagementId}/cadences/assigned-participants`);
  }

  addCadence(engagementId: number, request: CreateCommunicationCadence): Observable<CommunicationCadence> {
    return this.http.post<CommunicationCadence>(`/api/engagements/${engagementId}/cadences`, request);
  }
}
