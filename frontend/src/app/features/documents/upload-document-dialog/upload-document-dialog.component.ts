import { Component, DestroyRef, EventEmitter, Input, OnInit, Output, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../../services/document.service';
import { DocumentCategory, DocumentVisibility } from '../../../models/document.model';
import { DOCUMENT_MESSAGES, DOCUMENT_CONFIG } from '../../../shared/constants/document.constants';

@Component({
  selector: 'app-upload-document-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload-document-dialog.component.html',
  styleUrl: './upload-document-dialog.component.scss'
})
export class UploadDocumentDialogComponent implements OnInit {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) engagementId!: number;
  @Output() closed = new EventEmitter<void>();
  @Output() uploaded = new EventEmitter<void>();

  categories = signal<DocumentCategory[]>([]);
  selectedCategoryId = signal<number | null>(null);

  /** All files selected by the user */
  selectedFiles = signal<File[]>([]);

  /** Rename field — only relevant when exactly one file is selected */
  simulatedFileName = signal<string>('');

  /** True when only a single file is selected (rename is available) */
  isSingleFile = computed(() => this.selectedFiles().length === 1);

  selectedVisibility = signal<DocumentVisibility>(DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE);
  isUploading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.documentService.getCategories().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (cats) => {
        this.categories.set(cats);
        if (cats.length > 0) {
          this.selectedCategoryId.set(cats[0].id);
        }
      },
      error: () => {
        const defaults: DocumentCategory[] = [...DOCUMENT_CONFIG.DEFAULT_CATEGORIES];
        this.categories.set(defaults);
        this.selectedCategoryId.set(defaults[0].id);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const files = Array.from(input.files);
    this.selectedFiles.set(files);
    this.errorMessage.set(null);

    // Pre-fill rename field only for single file selection
    if (files.length === 1) {
      this.simulatedFileName.set(files[0].name);
    } else {
      this.simulatedFileName.set('');
    }
  }

  onSimulatedNameChange(name: string): void {
    this.simulatedFileName.set(name);
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
    this.closed.emit();
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

    // Resolve final files — apply rename only for single-file uploads
    const finalFiles: File[] = files.map((file) => {
      if (files.length === 1) {
        // Preserve original extension if user omitted it when renaming
        const originalExt = file.name.includes('.')
          ? file.name.substring(file.name.lastIndexOf('.')).toLowerCase()
          : '';
        const typedName = this.simulatedFileName().trim() || file.name;
        const finalName = typedName.includes('.') ? typedName : typedName + originalExt;
        return new File([file], finalName, { type: file.type });
      }
      return file;
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

    this.isUploading.set(true);
    this.errorMessage.set(null);

    this.documentService.uploadDocuments(finalFiles, this.engagementId, catId, this.selectedVisibility()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (results) => {
        this.isUploading.set(false);
        const failed = results.filter((r) => !r.success);
        if (failed.length > 0) {
          const msgs = failed.flatMap((r) => r.errors ?? [r.fileName ?? 'Unknown file']);
          this.errorMessage.set(msgs.join(', '));
        } else {
          this.uploaded.emit();
          this.closed.emit();
        }
      },
      error: (err) => {
        this.isUploading.set(false);
        this.errorMessage.set(this.extractErrorMessage(err));
      }
    });
  }

  private extractErrorMessage(err: any): string {
    if (err?.name === 'TypeError' || err?.status === 0 || (typeof err?.message === 'string' && err.message.toLowerCase().includes('fetch'))) {
      return DOCUMENT_MESSAGES.UPLOAD_NETWORK_OR_SERVER_LIMIT_ERROR;
    }
    const body = err?.error;
    if (Array.isArray(body)) {
      return body.flatMap((r: any) => r.errors || []).join(', ') || DOCUMENT_MESSAGES.UPLOAD_FAILED;
    }
    return body?.message || (typeof body === 'string' ? body : DOCUMENT_MESSAGES.UPLOAD_FAILED);
  }
}
