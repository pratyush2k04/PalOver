import { Component, DestroyRef, inject, signal } from '@angular/core';
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

export interface UploadVersionDialogData {
  documentId: number;
  file: File;
}

@Component({
  selector: 'app-upload-version-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule, AlertComponent, ButtonComponent, TextareaComponent],
  templateUrl: './upload-version-dialog.component.html',
  styleUrl: './upload-version-dialog.component.scss'
})
export class UploadVersionDialogComponent {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<boolean>>(DialogRef);
  readonly data = inject<UploadVersionDialogData>(DIALOG_DATA);

  changeNote = signal<string>('');
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  onChangeNoteChange(value: string): void {
    this.changeNote.set(value);
  }

  onClose(): void {
    if (!this.isSubmitting()) {
      this.dialogRef.close();
    }
  }

  onSubmit(): void {
    if (this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.documentService.uploadNewVersion(this.data.documentId, this.data.file, this.changeNote().trim()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const msg = typeof err.error === 'string'
          ? err.error
          : err.error?.message || DOCUMENT_MESSAGES.UPLOAD_VERSION_FAILED;
        this.errorMessage.set(msg);
      }
    });
  }
}
