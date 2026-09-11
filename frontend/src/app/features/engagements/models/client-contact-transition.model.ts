import { PersonDto } from '@shared/models/person.model';

export interface AddTransitionRequest {
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
}

export interface TransitionResponse {
  id: number;
  engagementId: number;
  outgoingContactName: string;
  outgoingContactRole: string;
  incomingContactName: string;
  incomingContactRole: string;
  createdAt?: string;
}

export interface AddClientContactTransitionDialogData {
  engagementId: number;
  activeTransitions: TransitionResponse[];
  people: PersonDto[];
}
