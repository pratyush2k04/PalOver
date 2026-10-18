import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddTransitionRequest, TransitionResponse } from '../models/client-contact-transition.model';
import { CLIENT_CONTACT_TRANSITION_ENDPOINTS } from '@shared/constants/client-contact-transition.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({ providedIn: 'root' })
export class ClientContactTransitionService {
  private http = inject(HttpClient);

  getTransitions(engagementId: number): Observable<TransitionResponse[]> {
    return this.http.get<TransitionResponse[]>(
      CLIENT_CONTACT_TRANSITION_ENDPOINTS.HISTORY(engagementId)
    );
  }

  saveTransition(engagementId: number, request: AddTransitionRequest, skipLoader = false): Observable<TransitionResponse> {
    return this.http.post<TransitionResponse>(
      CLIENT_CONTACT_TRANSITION_ENDPOINTS.BASE(engagementId),
      request,
      { context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined }
    );
  }
}
