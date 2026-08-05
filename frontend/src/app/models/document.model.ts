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
}

export interface DocumentCategory {
  id: number;
  categoryName: string;
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
