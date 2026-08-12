import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HANDOVER_API_ENDPOINTS } from '@shared/constants/handover.constants';

export interface SaveClientContactTransitionRequest {
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
}

export interface ClientContactTransitionResponseDto {
  clientContactTransitionId: number;
  engagementId: number;
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
}

export interface CurrentClientContactDto {
  name: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class ClientContactTransitionService {
  private http = inject(HttpClient);

  getTransition(engagementId: number): Observable<ClientContactTransitionResponseDto> {
    return this.http.get<ClientContactTransitionResponseDto>(
      HANDOVER_API_ENDPOINTS.CLIENT_CONTACT_TRANSITION(engagementId)
    );
  }

  getCurrentContact(engagementId: number): Observable<CurrentClientContactDto> {
    return this.http.get<CurrentClientContactDto>(
      HANDOVER_API_ENDPOINTS.CLIENT_CONTACT_TRANSITION_CURRENT(engagementId)
    );
  }

  saveTransition(
    engagementId: number,
    request: SaveClientContactTransitionRequest
  ): Observable<ClientContactTransitionResponseDto> {
    return this.http.post<ClientContactTransitionResponseDto>(
      HANDOVER_API_ENDPOINTS.CLIENT_CONTACT_TRANSITION(engagementId),
      request
    );
  }
}
