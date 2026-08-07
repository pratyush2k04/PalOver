import {
  Component,
  DestroyRef,
  inject,
  input,
  output,
  signal,
  computed
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { DocumentService } from '../../../services/document.service';
import { DOCUMENT_MESSAGES } from '../../../shared/constants/document.constants';

@Component({
  selector: 'app-remove-document-dialog',
  standalone: true,
  imports: [InlineSVGModule],
  templateUrl: './remove-document-dialog.component.html',
  styleUrl: './remove-document-dialog.component.scss'
})
export class RemoveDocumentDialogComponent {
  documentId = input.required<number>();
  documentName = input.required<string>();

  closed = output<void>();
  removed = output<void>();

  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);

  reason = signal<string>('');
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  trimmedReason = computed(() => this.reason().trim());
  isValidReason = computed(() => {
    const len = this.trimmedReason().length;
    return len >= 10 && len <= 500;
  });

  onReasonChange(value: string): void {
    this.reason.set(value);
    if (this.errorMessage()) {
      this.errorMessage.set(null);
    }
  }

  onClose(): void {
    if (!this.isSubmitting()) {
      this.closed.emit();
    }
  }

  onSubmit(): void {
    if (!this.isValidReason() || this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.documentService.deleteDocument(this.documentId(), this.trimmedReason()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.removed.emit();
        this.closed.emit();
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
