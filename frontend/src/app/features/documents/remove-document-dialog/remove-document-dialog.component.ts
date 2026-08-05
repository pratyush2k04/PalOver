import {
  Component,
  Input,
  Output,
  EventEmitter,
  inject,
  signal,
  computed
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../../services/document.service';

@Component({
  selector: 'app-remove-document-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './remove-document-dialog.component.html',
  styleUrl: './remove-document-dialog.component.scss'
})
export class RemoveDocumentDialogComponent {
  @Input({ required: true }) documentId!: number;
  @Input({ required: true }) documentName!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() removed = new EventEmitter<void>();

  private documentService = inject(DocumentService);

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

    this.documentService.deleteDocument(this.documentId, this.trimmedReason()).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.removed.emit();
        this.closed.emit();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const msg = typeof err.error === 'string' 
          ? err.error 
          : err.error?.message || 'Failed to remove document.';
        this.errorMessage.set(msg);
      }
    });
  }
}
