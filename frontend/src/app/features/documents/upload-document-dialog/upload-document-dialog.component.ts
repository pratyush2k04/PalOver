import { Component, DestroyRef, EventEmitter, Input, OnInit, Output, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../../services/document.service';
import { DocumentCategory } from '../../../models/document.model';

const ALLOWED_EXTENSIONS = ['.pdf', '.docx', '.xlsx', '.pptx', '.png', '.jpg', '.jpeg', '.msg', '.eml'];
const MAX_FILE_SIZE_BYTES = 50 * 1024 * 1024; // 50 MB

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

  selectedVisibility = signal<string>('Team-wide');
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
        const defaults: DocumentCategory[] = [
          { id: 1, categoryName: 'RFP' },
          { id: 2, categoryName: 'Proposal' },
          { id: 3, categoryName: 'Signed SOW' },
          { id: 4, categoryName: 'Email Thread' },
          { id: 5, categoryName: 'Design or Approach' },
          { id: 7, categoryName: 'Commercial' },
          { id: 6, categoryName: 'Other' }
        ];
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
    const cat = this.categories().find(c => c.id === id);
    if (cat && (cat.categoryName === 'Commercial' || cat.categoryName === 'Proposal')) {
      this.selectedVisibility.set('PM-only');
    } else {
      this.selectedVisibility.set('Team-wide');
    }
  }

  onVisibilityChange(visibility: string): void {
    this.selectedVisibility.set(visibility);
  }

  onClose(): void {
    this.closed.emit();
  }

  onSubmit(): void {
    const files = this.selectedFiles();
    const catId = this.selectedCategoryId();

    if (files.length === 0) {
      this.errorMessage.set('Please select at least one file to upload.');
      return;
    }

    if (!catId) {
      this.errorMessage.set('Please select a category.');
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
      if (!ext || !ALLOWED_EXTENSIONS.includes(ext)) {
        this.errorMessage.set(`"${f.name}" is not a supported file type.`);
        return;
      }
      if (f.size > MAX_FILE_SIZE_BYTES) {
        this.errorMessage.set(`"${f.name}" exceeds the 50 MB size limit.`);
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
      return 'File exceeds server upload size limit or network connection failed.';
    }
    const body = err?.error;
    if (Array.isArray(body)) {
      return body.flatMap((r: any) => r.errors || []).join(', ') || 'Failed to upload document.';
    }
    return body?.message || (typeof body === 'string' ? body : 'Failed to upload document.');
  }
}
