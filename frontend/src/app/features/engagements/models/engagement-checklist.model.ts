export interface MandatoryIncompleteItem {
  id: number;
  title: string;
  status: string;
  categoryName: string;
}

export interface EngagementChecklistItem {
  id: number;
  engagementChecklistId: number;
  sourceMasterItemId: number | null;
  title: string;
  description: string | null;
  categoryId: number;
  categoryName: string;
  isMandatory: boolean;
  displayOrder: number;
  isApplicable: boolean;
  status: string;
  notApplicableReason: string | null;
  exemptionStatus: 'None' | 'Pending' | 'Approved' | 'Rejected' | string;
  exemptionRequestedById: number | null;
  exemptionRequestedAt: string | null;
  exemptionReviewedById: number | null;
  exemptionReviewedAt: string | null;
  exemptionRejectionReason: string | null;
  ownerId: number | null;
  ownerName: string | null;
  blockReason: string | null;
  evidenceRequired: boolean;
  evidenceDocumentId: number | null;
  evidenceDocumentFileName: string | null;
  evidenceSubmittedAt: string | null;
  evidenceSubmittedById: number | null;
  evidenceSubmittedByName: string | null;
}

export interface EngagementChecklistCategoryGroup {
  categoryId: number;
  categoryName: string;
  items: EngagementChecklistItem[];
}

export interface EngagementChecklist {
  id: number;
  engagementId: number;
  sourceMasterVersionId: number;
  versionNumber: number;
  pendingExemptionsCount: number;
  incompleteMandatoryItems: MandatoryIncompleteItem[];
  categories: EngagementChecklistCategoryGroup[];
}

export interface UpdateItemStatusRequest {
  status: 'Complete' | 'Incomplete' | string;
}

export interface UpdateItemApplicabilityRequest {
  isApplicable: boolean;
  reason?: string;
}

export interface ReviewExemptionRequest {
  approved: boolean;
  rejectionReason?: string;
}

export interface UpdateItemOwnerRequest {
  personId: number | null;
}

export interface BlockChecklistItemRequest {
  reason: string;
  personId: number;
}

export interface SaveChecklistItemEvidenceRequest {
  documentId: number;
  markComplete?: boolean;
}
