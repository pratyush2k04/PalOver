import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { InputComponent } from '@common/components/form/input/input';
import { DocumentService } from '../../../services/document.service';
import { DOCUMENT_MESSAGES } from '@shared/constants/document.constants';

export interface UploadVersionDialogData {
  documentId: number;
  file: File;
}

export type UploadVersionDialogResult = true;

@Component({
  selector: 'app-upload-version-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AlertComponent, TextareaComponent, InputComponent],
  templateUrl: './upload-version-dialog.component.html',
  styleUrl: './upload-version-dialog.component.scss'
})
export class UploadVersionDialogComponent {
  private dialogRef = inject<DialogRef<UploadVersionDialogResult>>(DialogRef);
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  readonly data = inject<UploadVersionDialogData>(DIALOG_DATA);

  changeNote = signal<string>('');
  displayName = signal<string>(this.data.file.name);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  onChangeNoteChange(value: string): void {
    this.changeNote.set(value);
  }

  onDisplayNameChange(value: string): void {
    this.displayName.set(value);
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    const originalExt = this.data.file.name.includes('.')
      ? this.data.file.name.substring(this.data.file.name.lastIndexOf('.')).toLowerCase()
      : '';
    const typedName = this.displayName().trim() || this.data.file.name;
    const finalName = typedName.includes('.') ? typedName : typedName + originalExt;
    const finalFile = new File([this.data.file], finalName, { type: this.data.file.type });

    this.documentService
      .uploadNewVersion(this.data.documentId, finalFile, this.changeNote().trim(), true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(err?.error?.message || err?.error || DOCUMENT_MESSAGES.UPLOAD_VERSION_FAILED);
        },
      });
  }
}
