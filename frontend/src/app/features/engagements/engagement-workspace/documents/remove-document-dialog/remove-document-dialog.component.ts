import {
  Component,
  inject,
  signal,
  computed
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface RemoveDocumentDialogData {
  documentId: number;
  documentName: string;
}

export interface RemoveDocumentDialogResult {
  reason: string;
}

@Component({
  selector: 'app-remove-document-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './remove-document-dialog.component.html',
  styleUrl: './remove-document-dialog.component.scss'
})
export class RemoveDocumentDialogComponent {
  private dialogRef = inject<DialogRef<RemoveDocumentDialogResult>>(DialogRef);
  readonly data = inject<RemoveDocumentDialogData>(DIALOG_DATA);

  reason = signal<string>('');

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
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.isValidReason()) return;
    this.dialogRef.close({ reason: this.trimmedReason() });
  }
}
