import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface UploadVersionDialogData {
  documentId: number;
  file: File;
}

export interface UploadVersionDialogResult {
  changeNote: string;
}

@Component({
  selector: 'app-upload-version-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './upload-version-dialog.component.html',
  styleUrl: './upload-version-dialog.component.scss'
})
export class UploadVersionDialogComponent {
  private dialogRef = inject<DialogRef<UploadVersionDialogResult>>(DialogRef);
  readonly data = inject<UploadVersionDialogData>(DIALOG_DATA);

  changeNote = signal<string>('');

  onChangeNoteChange(value: string): void {
    this.changeNote.set(value);
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    this.dialogRef.close({ changeNote: this.changeNote().trim() });
  }
}
