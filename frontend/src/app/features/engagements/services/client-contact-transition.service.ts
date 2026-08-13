import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddTransitionRequest, TransitionResponse } from '../models/client-contact-transition.model';
import { CLIENT_CONTACT_TRANSITION_ENDPOINTS } from '@shared/constants/client-contact-transition.constants';

@Injectable({ providedIn: 'root' })
export class ClientContactTransitionService {
  private http = inject(HttpClient);

  getTransitions(engagementId: number): Observable<TransitionResponse[]> {
    return this.http.get<TransitionResponse[]>(
      CLIENT_CONTACT_TRANSITION_ENDPOINTS.HISTORY(engagementId)
    );
  }

  saveTransition(engagementId: number, request: AddTransitionRequest): Observable<TransitionResponse> {
    return this.http.post<TransitionResponse>(
      CLIENT_CONTACT_TRANSITION_ENDPOINTS.BASE(engagementId),
      request
    );
  }
}
