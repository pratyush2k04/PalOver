import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DocumentCategory, DocumentItem, DocumentUploadResult, DocumentVersion, DocumentVisibility, UpdateDocumentVisibilityRequest } from '../models/document.model';
import { DOCUMENT_API_ENDPOINTS } from '@shared/constants/document.constants';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);

  getEngagementDocuments(engagementId: number): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(DOCUMENT_API_ENDPOINTS.BY_ENGAGEMENT(engagementId));
  }

  getCategories(): Observable<DocumentCategory[]> {
    return this.http.get<DocumentCategory[]>(DOCUMENT_API_ENDPOINTS.CATEGORIES);
  }

  uploadDocument(file: File, engagementId: number, categoryId: number, visibility: DocumentVisibility): Observable<DocumentUploadResult[]> {
    return this.uploadDocuments([file], engagementId, categoryId, visibility);
  }

  uploadDocuments(files: File[], engagementId: number, categoryId: number, visibility: DocumentVisibility): Observable<DocumentUploadResult[]> {
    const formData = new FormData();
    files.forEach((file, i) => {
      formData.append(`[${i}].File`, file, file.name);
      formData.append(`[${i}].EngagementId`, engagementId.toString());
      formData.append(`[${i}].CategoryId`, categoryId.toString());
      formData.append(`[${i}].Visibility`, visibility);
    });
    return this.http.post<DocumentUploadResult[]>(DOCUMENT_API_ENDPOINTS.UPLOAD, formData);
  }

  downloadDocument(id: number, fileName: string): Observable<void> {
    return this.http.get(DOCUMENT_API_ENDPOINTS.DOWNLOAD(id), { responseType: 'blob' }).pipe(
      map((blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      })
    );
  }

  deleteDocument(id: number, reason: string): Observable<void> {
    return this.http.post<void>(DOCUMENT_API_ENDPOINTS.REMOVE(id), { reason });
  }

  updateVisibility(id: number, request: UpdateDocumentVisibilityRequest | DocumentVisibility): Observable<DocumentItem> {
    const body: UpdateDocumentVisibilityRequest =
      typeof request === 'string'
        ? { visibility: request as DocumentVisibility }
        : request;
    return this.http.patch<DocumentItem>(DOCUMENT_API_ENDPOINTS.VISIBILITY(id), body);
  }

  getVersionHistory(documentId: number): Observable<DocumentVersion[]> {
    return this.http.get<DocumentVersion[]>(DOCUMENT_API_ENDPOINTS.VERSIONS(documentId));
  }

  uploadNewVersion(documentId: number, file: File, changeNote: string): Observable<DocumentItem> {
    const formData = new FormData();
    formData.append('File', file, file.name);
    if (changeNote) {
      formData.append('ChangeNote', changeNote);
    }
    return this.http.post<DocumentItem>(DOCUMENT_API_ENDPOINTS.VERSIONS(documentId), formData);
  }

  downloadVersion(documentId: number, versionId: number, fileName: string): Observable<void> {
    return this.http.get(DOCUMENT_API_ENDPOINTS.DOWNLOAD_VERSION(documentId, versionId), { responseType: 'blob' }).pipe(
      map((blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      })
    );
  }
}
