import {
  Component,
  OnInit,
  inject,
  signal,
  computed,
  effect
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem } from '../../../models/document.model';
import { UploadDocumentDialogComponent } from '../upload-document-dialog/upload-document-dialog.component';
import { RemoveDocumentDialogComponent } from '../remove-document-dialog/remove-document-dialog.component';

@Component({
  selector: 'app-engagement-documents',
  standalone: true,
  imports: [CommonModule, UploadDocumentDialogComponent, RemoveDocumentDialogComponent],
  templateUrl: './engagement-documents.component.html',
  styleUrl: './engagement-documents.component.scss'
})
export class EngagementDocumentsComponent {
  private documentService = inject(DocumentService);
  private route = inject(ActivatedRoute);

  private parentParamMap = toSignal(this.route.parent!.paramMap);
  
  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  documents = signal<DocumentItem[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  showUploadModal = signal<boolean>(false);
  selectedDocForRemoval = signal<DocumentItem | null>(null);
  downloadingId = signal<number | null>(null);

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadDocuments(id);
      } else {
        this.documents.set([]);
        this.isLoading.set(false);
      }
    });
  }

  loadDocuments(id?: number): void {
    const targetId = id ?? this.engagementId();
    if (!targetId) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.documentService.getEngagementDocuments(targetId).subscribe({
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

  openRemoveModal(doc: DocumentItem): void {
    this.selectedDocForRemoval.set(doc);
  }

  closeRemoveModal(): void {
    this.selectedDocForRemoval.set(null);
  }

  onDocumentRemoved(): void {
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
