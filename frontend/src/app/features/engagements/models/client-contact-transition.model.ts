import { PersonDto } from '@shared/models/person.model';

export interface AddTransitionRequest {
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
  incomingContactEmail?: string | null;
  incomingContactBackground?: string | null;
  incomingContactSentiment?: string | null;
}

export interface TransitionResponse {
  id: number;
  engagementId: number;
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
  incomingContactEmail?: string | null;
  incomingContactBackground?: string | null;
  incomingContactSentiment?: string | null;
  createdAt?: string;
}

export interface AddClientContactTransitionDialogData {
  engagementId: number;
  activeTransitions: TransitionResponse[];
  people: PersonDto[];
}
