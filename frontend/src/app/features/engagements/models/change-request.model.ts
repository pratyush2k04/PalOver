export interface ChangeRequest {
  id: number;
  description: string;
  impact: string;
  status: string; // "Under Review" | "Approved" | "Rejected"
  rejectionReason?: string;
}

export interface CreateChangeRequest {
  description: string;
  impact: string;
}

export interface RejectChangeRequest {
  rejectionReason: string;
}
