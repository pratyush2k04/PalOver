import {
  Component,
  OnChanges,
  OnInit,
  SimpleChanges,
  inject,
  input,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';
import { UploadDocumentDialogComponent } from '../upload-document-dialog/upload-document-dialog.component';

@Component({
  selector: 'app-engagement-documents',
  standalone: true,
  imports: [CommonModule, UploadDocumentDialogComponent],
  templateUrl: './engagement-documents.component.html',
  styleUrl: './engagement-documents.component.scss'
})
export class EngagementDocumentsComponent implements OnInit, OnChanges {
  private documentService = inject(DocumentService);

  /** Signal input for the engagement ID */
  engagementId = input.required<number>();

  documents = signal<DocumentItem[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  showUploadModal = signal<boolean>(false);
  downloadingId = signal<number | null>(null);

  ngOnInit(): void {
    this.loadDocuments();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['engagementId'] && !changes['engagementId'].isFirstChange()) {
      this.loadDocuments();
    }
  }

  loadDocuments(): void {
    const id = this.engagementId();
    if (!id) {
      this.documents.set([]);
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.documentService.getEngagementDocuments(id).subscribe({
      next: (docs) => {
        this.documents.set(docs);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = typeof err.error === 'string' ? err.error : err.error?.message || 'Failed to load documents.';
        this.errorMessage.set(msg);
      }
    });
  }

  openUploadModal(): void {
    this.showUploadModal.set(true);
  }

  closeUploadModal(): void {
    this.showUploadModal.set(false);
  }

  onDocumentUploaded(): void {
    this.loadDocuments();
  }

  onDownload(doc: DocumentItem): void {
    this.downloadingId.set(doc.id);
    this.documentService.downloadDocument(doc.id, doc.fileName).subscribe({
      next: () => {
        this.downloadingId.set(null);
      },
      error: () => {
        this.downloadingId.set(null);
        alert(`Failed to download ${doc.fileName}`);
      }
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }
}
