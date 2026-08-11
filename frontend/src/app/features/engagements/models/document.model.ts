export type DocumentVisibility = 'PM-only' | 'Team-wide' | 'Custom';

export interface DocumentItem {
  id: number;
  engagementId: number;
  categoryId: number;
  categoryName: string;
  fileName: string;
  fileSizeBytes: number;
  contentType: string;
  uploadedBy: string;
  uploadedAt: string;
  visibility: DocumentVisibility;
  allowedRoles?: number[];
  isCustomVisibility?: boolean;
  categoryDefaultVisibility?: DocumentVisibility;
  currentVersionNumber: number;
}

export interface DocumentVersion {
  id: number;
  versionNumber: number;
  isCurrent: boolean;
  uploadedBy: string;
  uploadedAt: string;
  changeNote: string | null;
}

export interface DocumentCategory {
  id: number;
  categoryName: string;
}

export enum DocumentCategoryId {
  Rfp = 1,
  Proposal = 2,
  SignedSow = 3,
  EmailThread = 4,
  DesignOrApproach = 5,
  Other = 6,
  Commercial = 7
}

export interface DocumentUploadResult {
  fileName?: string;
  success: boolean;
  document?: DocumentItem;
  errors?: string[];
}

export interface RemoveDocumentRequest {
  reason: string;
}

export interface UpdateDocumentVisibilityRequest {
  visibility?: DocumentVisibility;
  allowedRoles?: number[];
  resetToCategoryDefault?: boolean;
}
