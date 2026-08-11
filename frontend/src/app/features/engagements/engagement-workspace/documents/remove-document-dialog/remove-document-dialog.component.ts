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
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DocumentService } from '../../../services/document.service';
import { DOCUMENT_MESSAGES } from '@shared/constants/document.constants';

export interface RemoveDocumentDialogData {
  documentId: number;
  documentName: string;
}

@Component({
  selector: 'app-remove-document-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule, AlertComponent, ButtonComponent, TextareaComponent],
  templateUrl: './remove-document-dialog.component.html',
  styleUrl: './remove-document-dialog.component.scss'
})
export class RemoveDocumentDialogComponent {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<boolean>>(DialogRef);
  readonly data = inject<RemoveDocumentDialogData>(DIALOG_DATA);

  reason = signal<string>('');
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  trimmedReason = computed(() => this.reason().trim());
  isValidReason = computed(() => {
    const len = this.trimmedReason().length;
    return len >= 10 && len <= 500;
  });
  minLengthError = computed(() => {
    const len = this.trimmedReason().length;
    return len > 0 && len < 10 ? 'Reason must be at least 10 characters long.' : null;
  });

  onReasonChange(value: string): void {
    this.reason.set(value);
    if (this.errorMessage()) {
      this.errorMessage.set(null);
    }
  }

  onClose(): void {
    if (!this.isSubmitting()) {
      this.dialogRef.close();
    }
  }

  onSubmit(): void {
    if (!this.isValidReason() || this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.documentService.deleteDocument(this.data.documentId, this.trimmedReason()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const msg = typeof err.error === 'string' 
          ? err.error 
          : err.error?.message || DOCUMENT_MESSAGES.REMOVE_FAILED;
        this.errorMessage.set(msg);
      }
    });
  }
}
