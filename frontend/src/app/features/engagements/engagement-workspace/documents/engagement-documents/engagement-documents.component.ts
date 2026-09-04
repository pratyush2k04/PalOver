import {
  Component,
  DestroyRef,
  OnInit,
  viewChild,
  ElementRef,
  inject,
  signal,
  computed,
  effect,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DocumentService } from '../../../services/document.service';
import { EngagementService } from '../../../services/engagement.service';
import { DocumentCategory, DocumentItem, DocumentVisibility } from '../../../models/document.model';
import { AuthService } from '@shared/services/auth';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  UploadDocumentDialogComponent,
  UploadDocumentDialogData,
  UploadDocumentDialogResult,
} from '../upload-document-dialog/upload-document-dialog.component';
import {
  RemoveDocumentDialogComponent,
  RemoveDocumentDialogData,
  RemoveDocumentDialogResult,
} from '../remove-document-dialog/remove-document-dialog.component';
import {
  CustomVisibilityDialogComponent,
  CustomVisibilityDialogData,
  CustomVisibilityDialogResult,
} from '../custom-visibility-dialog/custom-visibility-dialog.component';
import {
  UploadVersionDialogComponent,
  UploadVersionDialogData,
  UploadVersionDialogResult,
} from '../upload-version-dialog/upload-version-dialog.component';
import {
  VersionHistoryDialogComponent,
  VersionHistoryDialogData,
} from '../version-history-dialog/version-history-dialog.component';

import { DOCUMENT_CONFIG, DOCUMENT_MESSAGES } from '@shared/constants/document.constants';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

@Component({
  selector: 'app-engagement-documents',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    InlineSVGModule,
    SectionHeaderComponent,
    AlertComponent,
    ButtonComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    TooltipDirective,
    SelectComponent
  ],
  templateUrl: './engagement-documents.component.html',
  styleUrl: './engagement-documents.component.scss',
})
export class EngagementDocumentsComponent {
  private documentService = inject(DocumentService);
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  private dialog = inject(DialogService);

  readonly versionFileInput = viewChild<ElementRef<HTMLInputElement>>('versionFileInput');
  private pendingVersionDocId: number | null = null;

  readonly DOCUMENT_MESSAGES = DOCUMENT_MESSAGES;

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  documents = signal<DocumentItem[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  downloadingId = signal<number | null>(null);

  searchTerm = signal<string>('');
  selectedCategoryId = signal<number | 'all'>('all');
  categories = signal<DocumentCategory[]>([]);

  readonly visibilityOptions: SelectOption[] = [
    { value: 'Team-wide', label: 'Team-wide' },
    { value: 'PM-only', label: 'PM-only' },
    { value: 'Custom', label: 'Custom visibility' }
  ];

  categoryFilterOptions = computed(() => {
    return [
      { value: 'all', label: 'All categories' },
      ...this.categories().map((c) => ({ value: c.id, label: c.categoryName }))
    ];
  });

  filteredDocuments = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const categoryId = this.selectedCategoryId();
    return this.documents().filter(
      (doc) =>
        (categoryId === 'all' || doc.categoryId === categoryId) &&
        (!term || doc.fileName.toLowerCase().includes(term)),
    );
  });

  private engagementService = inject(EngagementService);

  readonly isSignedOff = computed(() => this.engagementService.isSignedOff());
  readonly canUpload = computed(() => !this.isSignedOff());

  /** True for PM and Admin — can edit visibility inline */
  canManageVisibility = computed(
    () => !this.isSignedOff() && (this.authService.hasRole('Project Management') || this.authService.isAdmin()),
  );

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadDocuments(id);

        if (this.route.snapshot.queryParamMap.get('action') === 'create') {
          this.openUploadModal();
        }
      } else {
        this.documents.set([]);
        this.isLoading.set(false);
      }
    });
    this.loadCategories();
  }

  private loadCategories(): void {
    this.documentService
      .getCategories()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (cats) => this.categories.set(cats),
        error: () => this.categories.set([...DOCUMENT_CONFIG.DEFAULT_CATEGORIES]),
      });
  }

  loadDocuments(id?: number, onDone?: () => void): void {
    const targetId = id ?? this.engagementId();
    if (!targetId) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.documentService
      .getEngagementDocuments(targetId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (docs) => {
          this.documents.set(docs);
          this.isLoading.set(false);
          onDone?.();
        },
        error: (err) => {
          this.isLoading.set(false);
          const msg =
            typeof err.error === 'string'
              ? err.error
              : err.error?.message || DOCUMENT_MESSAGES.LOAD_FAILED;
          this.errorMessage.set(msg);
          onDone?.();
        },
      });
  }

  openUploadModal(): void {
    const engagementId = this.engagementId();
    if (!engagementId) return;

    const dialogRef = this.dialog.open<
      UploadDocumentDialogComponent,
      UploadDocumentDialogData,
      UploadDocumentDialogResult
    >(UploadDocumentDialogComponent, { data: { engagementId } });

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.documentService
        .uploadDocuments(result.files, engagementId, result.categoryId, result.visibility, true)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (results) => {
            const failed = results.filter((r) => !r.success);
            this.loadDocuments(undefined, () => {
              if (failed.length > 0) {
                const msgs = failed.flatMap((r) => r.errors ?? [r.fileName ?? 'Unknown file']);
                this.errorMessage.set(msgs.join(', '));
              }
            });
          },
          error: (err) => {
            this.errorMessage.set(this.extractUploadErrorMessage(err));
          },
        });
    });
  }

  private extractUploadErrorMessage(err: any): string {
    if (err?.name === 'TypeError' || err?.status === 0 || (typeof err?.message === 'string' && err.message.toLowerCase().includes('fetch'))) {
      return DOCUMENT_MESSAGES.UPLOAD_NETWORK_OR_SERVER_LIMIT_ERROR;
    }
    const body = err?.error;
    if (Array.isArray(body)) {
      return body.flatMap((r: any) => r.errors || []).join(', ') || DOCUMENT_MESSAGES.UPLOAD_FAILED;
    }
    return body?.message || (typeof body === 'string' ? body : DOCUMENT_MESSAGES.UPLOAD_FAILED);
  }

  openRemoveModal(doc: DocumentItem): void {
    const dialogRef = this.dialog.open<
      RemoveDocumentDialogComponent,
      RemoveDocumentDialogData,
      RemoveDocumentDialogResult
    >(RemoveDocumentDialogComponent, { data: { documentId: doc.id, documentName: doc.fileName } });

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.documentService
        .deleteDocument(doc.id, result.reason, true)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.loadDocuments();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : err.error?.message || DOCUMENT_MESSAGES.REMOVE_FAILED;
            this.errorMessage.set(msg);
          },
        });
    });
  }

  onDownload(doc: DocumentItem): void {
    this.downloadingId.set(doc.id);
    this.documentService
      .downloadDocument(doc.id, doc.fileName)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.downloadingId.set(null);
        },
        error: () => {
          this.downloadingId.set(null);
          alert(DOCUMENT_MESSAGES.DOWNLOAD_FAILED(doc.fileName));
        },
      });
  }

  openCustomVisibilityModal(doc: DocumentItem): void {
    const dialogRef = this.dialog.open<
      CustomVisibilityDialogComponent,
      CustomVisibilityDialogData,
      CustomVisibilityDialogResult
    >(CustomVisibilityDialogComponent, { data: { document: doc } });

    dialogRef.closed$.subscribe((result) => {
      if (!result) {
        // Trigger update to re-sync select value if modal was cancelled
        this.documents.update((docs) => [...docs]);
        return;
      }

      const payload =
        result.allowedRoles === null
          ? { resetToCategoryDefault: true }
          : { visibility: 'Custom' as DocumentVisibility, allowedRoles: result.allowedRoles, resetToCategoryDefault: false };

      this.documentService
        .updateVisibility(doc.id, payload, true)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (updatedDoc) => {
            this.documents.update((docs) => docs.map((d) => (d.id === updatedDoc.id ? updatedDoc : d)));
          },
          error: (err) => {
            this.documents.update((docs) => [...docs]);
            const msg =
              err.error?.message ||
              (result.allowedRoles === null
                ? DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_REMOVE_FAILED
                : DOCUMENT_MESSAGES.CUSTOM_VISIBILITY_UPDATE_FAILED);
            this.errorMessage.set(msg);
          },
        });
    });
  }

  onVisibilityChange(
    doc: DocumentItem,
    newVisibility: unknown,
  ): void {
    const vis = newVisibility as DocumentVisibility;
    if (vis === 'Custom') {
      this.openCustomVisibilityModal(doc);
      return;
    }

    // Optimistic update
    this.documents.update((docs) =>
      docs.map((d) =>
        d.id === doc.id ? { ...d, visibility: vis, allowedRoles: [] } : d,
      ),
    );

    this.documentService
      .updateVisibility(doc.id, vis)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (updated) => {
          this.documents.update((docs) => docs.map((d) => (d.id === updated.id ? updated : d)));
        },
        error: (err) => {
          // Revert on failure
          this.documents.update((docs) =>
            docs.map((d) => (d.id === doc.id ? { ...d, visibility: doc.visibility } : d)),
          );
          const msg = err.error?.message || DOCUMENT_MESSAGES.UPDATE_VISIBILITY_FAILED;
          this.errorMessage.set(msg);
        },
      });
  }

  triggerVersionUpload(doc: DocumentItem): void {
    this.pendingVersionDocId = doc.id;
    this.versionFileInput()?.nativeElement.click();
  }

  onVersionFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const documentId = this.pendingVersionDocId;
    this.pendingVersionDocId = null;

    if (!input.files || input.files.length === 0 || !documentId) {
      input.value = '';
      return;
    }

    const file = input.files[0];
    input.value = '';

    const dialogRef = this.dialog.open<
      UploadVersionDialogComponent,
      UploadVersionDialogData,
      UploadVersionDialogResult
    >(UploadVersionDialogComponent, { data: { documentId, file } });

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.documentService
        .uploadNewVersion(documentId, file, result.changeNote, true)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.loadDocuments();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : err.error?.message || DOCUMENT_MESSAGES.UPLOAD_VERSION_FAILED;
            this.errorMessage.set(msg);
          },
        });
    });
  }

  openVersionHistoryModal(doc: DocumentItem): void {
    this.dialog.open<VersionHistoryDialogComponent, VersionHistoryDialogData, void>(
      VersionHistoryDialogComponent,
      { data: { documentId: doc.id, fileName: doc.fileName } },
    );
  }
}
