import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DocumentService } from '../../../services/document.service';
import { DocumentCategory } from '../../../models/document.model';

@Component({
  selector: 'app-upload-document-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload-document-dialog.component.html',
  styleUrl: './upload-document-dialog.component.scss'
})
export class UploadDocumentDialogComponent implements OnInit {
  private documentService = inject(DocumentService);

  @Input({ required: true }) engagementId!: number;
  @Output() closed = new EventEmitter<void>();
  @Output() uploaded = new EventEmitter<void>();

  categories = signal<DocumentCategory[]>([]);
  selectedCategoryId = signal<number | null>(null);
  selectedFile = signal<File | null>(null);
  simulatedFileName = signal<string>('');
  selectedVisibility = signal<string>('Team-wide');
  isUploading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.documentService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        if (cats.length > 0) {
          this.selectedCategoryId.set(cats[0].id);
        }
      },
      error: () => {
        // Fallback default categories if server returns error or empty
        const defaults: DocumentCategory[] = [
          { id: 1, categoryName: 'Signed SOW' },
          { id: 2, categoryName: 'Proposal' },
          { id: 3, categoryName: 'Design or Approach' },
          { id: 4, categoryName: 'Email Thread' },
          { id: 5, categoryName: 'Other' }
        ];
        this.categories.set(defaults);
        this.selectedCategoryId.set(defaults[4].id);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.selectedFile.set(file);
      this.simulatedFileName.set(file.name);
      this.errorMessage.set(null);
    }
  }

  onSimulatedNameChange(name: string): void {
    this.simulatedFileName.set(name);
  }

  onCategoryChange(catId: string): void {
    const id = Number(catId);
    this.selectedCategoryId.set(id);
    // Auto-default visibility based on category
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
    const file = this.selectedFile();
    const catId = this.selectedCategoryId();

    if (!file) {
      this.errorMessage.set('Please select a file to upload.');
      return;
    }

    if (!catId) {
      this.errorMessage.set('Please select a category.');
      return;
    }

    // Preserve the original file extension if the user left it out when renaming
    const originalExt = file.name.includes('.')
      ? file.name.substring(file.name.lastIndexOf('.')).toLowerCase()
      : '';
    const typedName = this.simulatedFileName().trim() || file.name;
    const finalName = typedName.includes('.')
      ? typedName
      : typedName + originalExt;
    const finalFile = new File([file], finalName, { type: file.type });

    // Client-side pre-validation for allowed extension & size
    const allowedExtensions = ['.pdf', '.docx', '.xlsx', '.pptx', '.png', '.jpg', '.jpeg', '.msg', '.eml'];
    const ext = finalName.includes('.') ? finalName.substring(finalName.lastIndexOf('.')).toLowerCase() : '';
    if (!ext || !allowedExtensions.includes(ext)) {
      this.errorMessage.set('That file type is not supported.');
      return;
    }

    const maxFileSizeBytes = 50 * 1024 * 1024; // 50MB
    if (finalFile.size > maxFileSizeBytes) {
      this.errorMessage.set('Files must be 50 MB or smaller.');
      return;
    }

    this.isUploading.set(true);
    this.errorMessage.set(null);

    this.documentService.uploadDocument(finalFile, this.engagementId, catId, this.selectedVisibility()).subscribe({
      next: (results) => {
        this.isUploading.set(false);
        const failed = results.find((r) => !r.success);
        if (failed && failed.errors && failed.errors.length > 0) {
          this.errorMessage.set(failed.errors.join(', '));
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
