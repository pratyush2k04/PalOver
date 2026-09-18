import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DocumentCategory, DocumentItem, DocumentUploadResult, DocumentVersion, DocumentVisibility, UpdateDocumentVisibilityRequest } from '../models/document.model';
import { DOCUMENT_API_ENDPOINTS } from '@shared/constants/document.constants';
import { SKIP_LOADER } from '@shared/interceptors/loader-interceptor';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);

  getEngagementDocuments(engagementId: number, skipLoader = false): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(DOCUMENT_API_ENDPOINTS.BY_ENGAGEMENT(engagementId), {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  getCategories(skipLoader = false): Observable<DocumentCategory[]> {
    return this.http.get<DocumentCategory[]>(DOCUMENT_API_ENDPOINTS.CATEGORIES, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  uploadDocuments(files: File[], engagementId: number, categoryId: number, visibility: DocumentVisibility, skipLoader = false): Observable<DocumentUploadResult[]> {
    const formData = new FormData();
    files.forEach((file, i) => {
      formData.append(`[${i}].File`, file, file.name);
      formData.append(`[${i}].CategoryId`, categoryId.toString());
      formData.append(`[${i}].Visibility`, visibility);
    });
    return this.http.post<DocumentUploadResult[]>(DOCUMENT_API_ENDPOINTS.UPLOAD(engagementId), formData, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
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

  deleteDocument(id: number, reason: string, skipLoader = false): Observable<void> {
    return this.http.post<void>(DOCUMENT_API_ENDPOINTS.REMOVE(id), { reason }, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  updateVisibility(id: number, request: UpdateDocumentVisibilityRequest | DocumentVisibility, skipLoader = false): Observable<DocumentItem> {
    const body: UpdateDocumentVisibilityRequest =
      typeof request === 'string'
        ? { visibility: request as DocumentVisibility }
        : request;
    return this.http.patch<DocumentItem>(DOCUMENT_API_ENDPOINTS.VISIBILITY(id), body, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
  }

  getVersionHistory(documentId: number): Observable<DocumentVersion[]> {
    return this.http.get<DocumentVersion[]>(DOCUMENT_API_ENDPOINTS.VERSIONS(documentId), {
      context: new HttpContext().set(SKIP_LOADER, true)
    });
  }

  uploadNewVersion(documentId: number, file: File, changeNote: string, skipLoader = false): Observable<DocumentItem> {
    const formData = new FormData();
    formData.append('File', file, file.name);
    if (changeNote) {
      formData.append('ChangeNote', changeNote);
    }
    return this.http.post<DocumentItem>(DOCUMENT_API_ENDPOINTS.VERSIONS(documentId), formData, {
      context: skipLoader ? new HttpContext().set(SKIP_LOADER, true) : undefined
    });
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
