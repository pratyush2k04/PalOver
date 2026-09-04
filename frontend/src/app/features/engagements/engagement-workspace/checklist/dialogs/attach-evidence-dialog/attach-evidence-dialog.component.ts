import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { EngagementChecklistItem } from '@features/engagements/models/engagement-checklist.model';
import { DocumentCategory, DocumentItem, DocumentVisibility } from '@features/engagements/models/document.model';
import { DocumentService } from '@features/engagements/services/document.service';
import { DOCUMENT_CONFIG, DOCUMENT_MESSAGES } from '@shared/constants/document.constants';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { InputComponent } from '@common/components/form/input/input';
import { TabsComponent, TabItem } from '@common/components/tabs/tabs';

export interface AttachEvidenceDialogData {
  item: EngagementChecklistItem;
  engagementId: number;
  isMandatoryCompletion?: boolean;
  isReadOnly?: boolean;
}

export interface AttachEvidenceDialogResult {
  documentId: number;
  markComplete?: boolean;
}

type EvidenceMode = 'upload' | 'select';

@Component({
  selector: 'app-attach-evidence-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AlertComponent, SelectComponent, InputComponent, TabsComponent],
  templateUrl: './attach-evidence-dialog.component.html',
  styleUrl: './attach-evidence-dialog.component.scss'
})
export class AttachEvidenceDialogComponent implements OnInit {
  private documentService = inject(DocumentService);
  private destroyRef = inject(DestroyRef);
  private dialogRef = inject<DialogRef<AttachEvidenceDialogResult>>(DialogRef);
  readonly data = inject<AttachEvidenceDialogData>(DIALOG_DATA);

  readonly isReadOnly = this.data.isReadOnly ?? false;
  readonly isMandatoryCompletion = this.data.isMandatoryCompletion ?? false;

  mode = signal<EvidenceMode>('upload');

  readonly evidenceTabs: TabItem[] = [
    { id: 'upload', label: 'Upload New' },
    { id: 'select', label: 'Select Existing' }
  ];

  // Upload New section state
  categories = signal<DocumentCategory[]>([]);
  selectedCategoryId = signal<number | null>(null);
  selectedVisibility = signal<DocumentVisibility>(DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE);
  selectedFile = signal<File | null>(null);
  renamedFileName = signal<string>('');
  isUploading = signal<boolean>(false);

  // Select Existing section state
  existingDocuments = signal<DocumentItem[]>([]);
  isLoadingDocuments = signal<boolean>(false);
  selectedDocumentId = signal<number | null>(null);

  errorMessage = signal<string | null>(null);
  isDownloading = signal<boolean>(false);

  readonly categoryOptions = computed<SelectOption[]>(() =>
    this.categories().map((cat) => ({ value: cat.id, label: cat.categoryName }))
  );

  readonly visibilityOptions: SelectOption[] = [
    { value: DOCUMENT_CONFIG.VISIBILITY.TEAM_WIDE, label: 'Team-wide' },
    { value: DOCUMENT_CONFIG.VISIBILITY.PM_ONLY, label: 'PM-only (restricted)' }
  ];

  get hasExistingEvidence(): boolean {
    return !!this.data.item.evidenceDocumentId;
  }

  get dialogTitle(): string {
    if (this.isReadOnly) {
      return 'Evidence Details';
    }
    if (this.isMandatoryCompletion) {
      return 'Evidence required to complete this item';
    }
    return this.hasExistingEvidence ? 'Replace Evidence' : 'Attach Evidence';
  }

  get submitButtonText(): string {
    if (this.isMandatoryCompletion) {
      return 'Save & Complete';
    }
    return 'Save Evidence';
  }

  ngOnInit(): void {
    if (this.isReadOnly) {
      return;
    }
    this.loadCategories();
    this.loadExistingDocuments();
  }

  private loadCategories(): void {
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

  private loadExistingDocuments(): void {
    this.isLoadingDocuments.set(true);
    this.documentService.getEngagementDocuments(this.data.engagementId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (docs) => {
        this.existingDocuments.set(docs);
        this.isLoadingDocuments.set(false);
      },
      error: () => {
        this.existingDocuments.set([]);
        this.isLoadingDocuments.set(false);
      }
    });
  }

  setMode(mode: EvidenceMode): void {
    this.mode.set(mode);
    this.errorMessage.set(null);
  }

  onTabChange(tabId: string): void {
    this.setMode(tabId as EvidenceMode);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;
    this.selectedFile.set(file);
    this.renamedFileName.set(file?.name ?? '');
    this.errorMessage.set(null);
  }

  onRenamedFileNameChange(name: string): void {
    this.renamedFileName.set(name);
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

  selectExistingDocument(doc: DocumentItem): void {
    this.selectedDocumentId.set(doc.id);
    this.errorMessage.set(null);
  }

  downloadCurrentEvidence(): void {
    const item = this.data.item;
    if (!item.evidenceDocumentId || this.isDownloading()) {
      return;
    }
    this.isDownloading.set(true);
    this.documentService.downloadDocument(item.evidenceDocumentId, item.evidenceDocumentFileName || 'evidence').subscribe({
      next: () => this.isDownloading.set(false),
      error: () => this.isDownloading.set(false)
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.isReadOnly) {
      this.dialogRef.close();
      return;
    }

    const markComplete = this.isMandatoryCompletion ? true : undefined;

    if (this.mode() === 'select') {
      const documentId = this.selectedDocumentId();
      if (!documentId) {
        this.errorMessage.set('Please select a document.');
        return;
      }
      this.dialogRef.close({ documentId, markComplete });
      return;
    }

    const selectedFile = this.selectedFile();
    const catId = this.selectedCategoryId();

    if (!selectedFile) {
      this.errorMessage.set('Please select a file to upload.');
      return;
    }
    if (!catId) {
      this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_NO_CATEGORY_SELECTED);
      return;
    }

    // Preserve original extension if the typed name omits it
    const originalExt = selectedFile.name.includes('.')
      ? selectedFile.name.substring(selectedFile.name.lastIndexOf('.')).toLowerCase()
      : '';
    const typedName = this.renamedFileName().trim() || selectedFile.name;
    const finalName = typedName.includes('.') ? typedName : typedName + originalExt;
    const file = new File([selectedFile], finalName, { type: selectedFile.type });

    const ext = file.name.includes('.') ? file.name.substring(file.name.lastIndexOf('.')).toLowerCase() : '';
    if (!ext || !DOCUMENT_CONFIG.ALLOWED_EXTENSIONS.includes(ext)) {
      this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_UNSUPPORTED_FILE_TYPE(file.name));
      return;
    }
    if (file.size > DOCUMENT_CONFIG.MAX_FILE_SIZE_BYTES) {
      this.errorMessage.set(DOCUMENT_MESSAGES.UPLOAD_FILE_EXCEEDS_SIZE_LIMIT(file.name));
      return;
    }

    this.isUploading.set(true);
    this.errorMessage.set(null);

    this.documentService.uploadDocuments([file], this.data.engagementId, catId, this.selectedVisibility())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (results) => {
          this.isUploading.set(false);
          const result = results[0];
          if (!result?.success || !result.document) {
            this.errorMessage.set(result?.errors?.join(', ') || DOCUMENT_MESSAGES.UPLOAD_FAILED);
            return;
          }
          this.dialogRef.close({ documentId: result.document.id, markComplete });
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
