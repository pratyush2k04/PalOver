import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DocumentCategory, DocumentItem, DocumentUploadResult } from '../models/document.model';
import { ENGAGEMENT_API_ENDPOINTS } from '../../../constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);

  getEngagementDocuments(engagementId: number): Observable<DocumentItem[]> {
    return this.http.get<DocumentItem[]>(ENGAGEMENT_API_ENDPOINTS.ENGAGEMENT_DOCUMENTS(engagementId));
  }

  getCategories(): Observable<DocumentCategory[]> {
    return this.http.get<DocumentCategory[]>(ENGAGEMENT_API_ENDPOINTS.DOCUMENT_CATEGORIES);
  }

  uploadDocument(file: File, engagementId: number, categoryId: number): Observable<DocumentUploadResult[]> {
    const formData = new FormData();
    formData.append('[0].File', file, file.name);
    formData.append('[0].EngagementId', engagementId.toString());
    formData.append('[0].CategoryId', categoryId.toString());

    return this.http.post<DocumentUploadResult[]>(ENGAGEMENT_API_ENDPOINTS.DOCUMENT_UPLOAD, formData);
  }

  downloadDocument(id: number, fileName: string): Observable<void> {
    return this.http.get(ENGAGEMENT_API_ENDPOINTS.DOCUMENT_DOWNLOAD(id), { responseType: 'blob' }).pipe(
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

  deleteDocument(id: number): Observable<void> {
    return this.http.delete<void>(ENGAGEMENT_API_ENDPOINTS.DOCUMENT_DELETE(id));
  }
}
