import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DocumentCategory, DocumentItem, DocumentUploadResult, UpdateDocumentVisibilityRequest } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);

  getEngagementDocuments(engagementId: number): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(`/api/documents/engagement/${engagementId}`);
  }

  getCategories(): Observable<DocumentCategory[]> {
    return this.http.get<DocumentCategory[]>('/api/documents/categories');
  }

  uploadDocument(file: File, engagementId: number, categoryId: number, visibility: string): Observable<DocumentUploadResult[]> {
    const formData = new FormData();
    formData.append('[0].File', file, file.name);
    formData.append('[0].EngagementId', engagementId.toString());
    formData.append('[0].CategoryId', categoryId.toString());
    formData.append('[0].Visibility', visibility);

    return this.http.post<DocumentUploadResult[]>('/api/documents/upload', formData);
  }

  downloadDocument(id: number, fileName: string): Observable<void> {
    return this.http.get(`/api/documents/${id}/download`, { responseType: 'blob' }).pipe(
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
    return this.http.post<void>(`/api/documents/${id}/remove`, { reason });
  }

  updateVisibility(id: number, visibility: string): Observable<DocumentItem> {
    const body: UpdateDocumentVisibilityRequest = { visibility: visibility as 'PM-only' | 'Team-wide' };
    return this.http.patch<DocumentItem>(`/api/documents/${id}/visibility`, body);
  }
}
