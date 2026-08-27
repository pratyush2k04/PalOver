import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { DocumentService } from '../../../services/document.service';
import { DocumentVersion } from '../../../models/document.model';
import { DOCUMENT_MESSAGES } from '@shared/constants/document.constants';

export interface VersionHistoryDialogData {
  documentId: number;
  fileName: string;
}

@Component({
  selector: 'app-version-history-dialog',
  standalone: true,
  imports: [CommonModule, AlertComponent, ButtonComponent, LoadingSpinnerComponent, StatusBadgeComponent],
  templateUrl: './version-history-dialog.component.html',
  styleUrl: './version-history-dialog.component.scss'
})
export class VersionHistoryDialogComponent implements OnInit {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<void>>(DialogRef);
  readonly data = inject<VersionHistoryDialogData>(DIALOG_DATA);

  versions = signal<DocumentVersion[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  downloadingId = signal<number | null>(null);

  ngOnInit(): void {
    this.documentService.getVersionHistory(this.data.documentId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (versions) => {
        this.versions.set(versions);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = typeof err.error === 'string' ? err.error : err.error?.message || DOCUMENT_MESSAGES.VERSION_HISTORY_LOAD_FAILED;
        this.errorMessage.set(msg);
      }
    });
  }

  onDownload(version: DocumentVersion): void {
    this.downloadingId.set(version.id);
    this.documentService.downloadVersion(this.data.documentId, version.id, this.data.fileName).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.downloadingId.set(null);
      },
      error: () => {
        this.downloadingId.set(null);
        this.errorMessage.set(DOCUMENT_MESSAGES.DOWNLOAD_VERSION_FAILED(this.data.fileName));
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

  onClose(): void {
    this.dialogRef.close();
  }
}
