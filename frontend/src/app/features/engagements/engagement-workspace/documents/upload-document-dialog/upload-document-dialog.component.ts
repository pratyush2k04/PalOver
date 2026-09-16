import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { DocumentService } from '../../../services/document.service';
import { DocumentCategory, DocumentCategoryId, DocumentVisibility } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '@shared/constants/document.constants';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

export interface UploadDocumentDialogData {
  engagementId: number;
  categories: DocumentCategory[];
}

export interface UploadDocumentDialogResult {
  success: true;
  /** Names of files that failed to upload, empty when the whole batch succeeded. */
  failedFileNames: string[];
}

@Component({
  selector: 'app-upload-document-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    AlertComponent,
    ButtonComponent,
    InputComponent,
    SelectComponent
  ],
  templateUrl: './upload-document-dialog.component.html',
  styleUrl: './upload-document-dialog.component.scss'
})
export class UploadDocumentDialogComponent {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<UploadDocumentDialogResult>>(DialogRef);
  private data = inject<UploadDocumentDialogData>(DIALOG_DATA);

  categories = signal<DocumentCategory[]>(this.data.categories);
  selectedCategoryId = signal<number | null>(
    this.data.categories.find((cat) => cat.id === DocumentCategoryId.Other)?.id
      ?? this.data.categories[0]?.id
      ?? null
  );

  /** All files selected by the user */
  selectedFiles = signal<File[]>([]);

  /** Rename field per selected file, same order as selectedFiles() */
  renamedFileNames = signal<string[]>([]);

  selectedVisibility = signal<DocumentVisibility>(DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  readonly categoryOptions = computed<SelectOption[]>(() =>
    this.categories().map((cat) => ({ value: cat.id, label: cat.categoryName }))
  );

  readonly visibilityOptions: SelectOption[] = [
    { value: DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE, label: 'Team-wide' },
    { value: DOCUMENT_CONFIG.VISIBILITY.PM_ONLY, label: 'PM-only (restricted)' }
  ];

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const files = Array.from(input.files);
    this.selectedFiles.set(files);
    this.renamedFileNames.set(files.map((f) => f.name));
    this.errorMessage.set(null);
  }

  onRenamedFileNameChange(index: number, name: string): void {
    this.renamedFileNames.update((names) => names.map((n, i) => (i === index ? name : n)));
  }

  onCategoryChange(catId: string): void {
    const id = Number(catId);
    this.selectedCategoryId.set(id);
    if (DOCUMENT_CONFIG.PM_ONLY_CATEGORY_IDS.includes(id)) {
      this.selectedVisibility.set(DOCUMENT_CONFIG.VISIBILITY.PM_ONLY);
    } else {
      this.selectedVisibility.set(DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE);
    }
  }

  onVisibilityChange(visibility: DocumentVisibility): void {
    this.selectedVisibility.set(visibility);
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const files = this.selectedFiles();
    const catId = this.selectedCategoryId();

    if (files.length === 0) {
      this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_NO_FILES_SELECTED);
      return;
    }

    if (!catId) {
      this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_NO_CATEGORY_SELECTED);
      return;
    }

    // Resolve final files — apply the per-file rename, preserving each file's own extension
    const renamedNames = this.renamedFileNames();
    const finalFiles: File[] = files.map((file, i) => {
      const originalExt = file.name.includes('.')
        ? file.name.substring(file.name.lastIndexOf('.')).toLowerCase()
        : '';
      const typedName = (renamedNames[i] ?? '').trim() || file.name;
      const finalName = typedName.includes('.') ? typedName : typedName + originalExt;
      return new File([file], finalName, { type: file.type });
    });

    // Client-side validation: extension + size for every file
    for (const f of finalFiles) {
      const ext = f.name.includes('.')
        ? f.name.substring(f.name.lastIndexOf('.')).toLowerCase()
        : '';
      if (!ext || !DOCUMENT_CONFIG.ALLOWED_EXTENSIONS.includes(ext)) {
        this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_UNSUPPORTED_FILE_TYPE(f.name));
        return;
      }
      if (f.size > DOCUMENT_CONFIG.MAX_FILE_SIZE_BYTES) {
        this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_FILE_EXCEEDS_SIZE_LIMIT(f.name));
        return;
      }
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.documentService
      .uploadDocuments(finalFiles, this.data.engagementId, catId, this.selectedVisibility(), true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (results) => {
          const failed = results.filter((r) => !r.success);

          if (results.length > 0 && failed.length === results.length) {
            // Entire batch failed — keep the dialog open so the user can retry.
            this.isSubmitting.set(false);
            const msgs = failed.flatMap((r) => r.errors ?? [r.fileName ?? 'Unknown file']);
            this.errorMessage.set(msgs.join(', ') || 'All files failed to upload.');
            return;
          }

          const failedFileNames = failed.flatMap((r) => r.errors ?? [r.fileName ?? 'Unknown file']);
          this.dialogRef.close({ success: true, failedFileNames });
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(this.extractUploadErrorMessage(err));
        },
      });
  }

  private extractUploadErrorMessage(err: any): string {
    if (
      err?.name === 'TypeError' ||
      err?.status === 0 ||
      (typeof err?.message === 'string' && err.message.toLowerCase().includes('fetch'))
    ) {
      return DOCUMENT_MESSAGES.UPLOAD_NETWORK_OR_SERVER_LIMIT_ERROR;
    }
    const body = err?.error;
    if (Array.isArray(body)) {
      return body.flatMap((r: any) => r.errors || []).join(', ') || DOCUMENT_MESSAGES.UPLOAD_FAILED;
    }
    return body?.message || (typeof body === 'string' ? body : DOCUMENT_MESSAGES.UPLOAD_FAILED);
  }
}
