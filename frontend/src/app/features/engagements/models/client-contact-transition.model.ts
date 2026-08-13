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
