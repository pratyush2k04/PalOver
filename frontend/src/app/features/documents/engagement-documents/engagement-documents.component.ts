import {
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal,
  computed,
  effect
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DocumentService } from '../../../services/document.service';
import { DocumentItem, DocumentVisibility } from '../../../models/document.model';
import { AuthService } from '../../../services/auth';
import { UploadDocumentDialogComponent } from '../upload-document-dialog/upload-document-dialog.component';
import { RemoveDocumentDialogComponent } from '../remove-document-dialog/remove-document-dialog.component';
import { CustomVisibilityDialogComponent } from '../custom-visibility-dialog/custom-visibility-dialog.component';

import { DOCUMENT_MESSAGES } from '../../../shared/constants/document.constants';
import { AlertComponent } from '../../../common/components/alert/alert';
import { ButtonComponent } from '../../../common/components/button/button';

@Component({
  selector: 'app-engagement-documents',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    UploadDocumentDialogComponent,
    RemoveDocumentDialogComponent,
    CustomVisibilityDialogComponent
  , AlertComponent, ButtonComponent],
  templateUrl: './engagement-documents.component.html',
  styleUrl: './engagement-documents.component.scss'
})
export class EngagementDocumentsComponent {
  private documentService = inject(DocumentService);
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);

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
  selectedDocForCustomVisibility = signal<DocumentItem | null>(null);
  downloadingId = signal<number | null>(null);

  /** True for PM and Admin — can edit visibility inline */
  canManageVisibility = computed(() =>
    this.authService.hasRole('Project Management') || this.authService.isAdmin()
  );

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

    this.documentService.getEngagementDocuments(targetId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (docs) => {
        this.documents.set(docs);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = typeof err.error === 'string' ? err.error : err.error?.message || DOCUMENT_MESSAGES.LOAD_FAILED;
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
    this.documentService.downloadDocument(doc.id, doc.fileName).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.downloadingId.set(null);
      },
      error: () => {
        this.downloadingId.set(null);
        alert(DOCUMENT_MESSAGES.DOWNLOAD_FAILED(doc.fileName));
      }
    });
  }

  openCustomVisibilityModal(doc: DocumentItem): void {
    this.selectedDocForCustomVisibility.set(doc);
  }

  closeCustomVisibilityModal(): void {
    this.selectedDocForCustomVisibility.set(null);
  }

  onCustomVisibilityUpdated(updatedDoc: DocumentItem): void {
    this.documents.update(docs =>
      docs.map(d => d.id === updatedDoc.id ? updatedDoc : d)
    );
    // Keep the signal fresh so re-opening the modal shows current allowedRoles
    if (this.selectedDocForCustomVisibility()?.id === updatedDoc.id) {
      this.selectedDocForCustomVisibility.set(updatedDoc);
    }
  }

  onVisibilityChange(doc: DocumentItem, newVisibility: DocumentVisibility): void {
    if (newVisibility === 'Custom') {
      this.openCustomVisibilityModal(doc);
      return;
    }

    // Optimistic update
    this.documents.update(docs =>
      docs.map(d => d.id === doc.id ? { ...d, visibility: newVisibility, allowedRoles: [] } : d)
    );

    this.documentService.updateVisibility(doc.id, newVisibility).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (updated) => {
        this.documents.update(docs =>
          docs.map(d => d.id === updated.id ? updated : d)
        );
      },
      error: (err) => {
        // Revert on failure
        this.documents.update(docs =>
          docs.map(d => d.id === doc.id ? { ...d, visibility: doc.visibility } : d)
        );
        const msg = err.error?.message || DOCUMENT_MESSAGES.UPDATE_VISIBILITY_FAILED;
        this.errorMessage.set(msg);
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
