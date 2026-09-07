import {
  Component,
  DestroyRef,
  inject,
  signal,
  computed
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DocumentService } from '../../../services/document.service';
import { DOCUMENT_MESSAGES } from '@shared/constants/document.constants';

export interface RemoveDocumentDialogData {
  documentId: number;
  documentName: string;
}

export type RemoveDocumentDialogResult = true;

@Component({
  selector: 'app-remove-document-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AlertComponent, TextareaComponent],
  templateUrl: './remove-document-dialog.component.html',
  styleUrl: './remove-document-dialog.component.scss'
})
export class RemoveDocumentDialogComponent {
  private dialogRef = inject<DialogRef<RemoveDocumentDialogResult>>(DialogRef);
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  readonly data = inject<RemoveDocumentDialogData>(DIALOG_DATA);

  reason = signal<string>('');
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  trimmedReason = computed(() => this.reason().trim());
  isValidReason = computed(() => {
    const len = this.trimmedReason().length;
    return len >= 10 && len <= 500;
  });
  reasonError = signal<string | null>(null);

  onReasonChange(value: string): void {
    this.reason.set(value);
    this.reasonError.set(null);
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.isValidReason()) {
      const len = this.trimmedReason().length;
      this.reasonError.set(
        len === 0 ? 'Reason for removal is required.' : 'Reason must be at least 10 characters long.'
      );
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.documentService
      .deleteDocument(this.data.documentId, this.trimmedReason(), true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(err?.error?.message || err?.error || DOCUMENT_MESSAGES.REMOVE_FAILED);
        },
      });
  }
}
