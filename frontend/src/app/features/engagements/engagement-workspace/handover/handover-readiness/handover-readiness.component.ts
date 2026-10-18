import { Component, OnInit, inject, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HandoverReadinessService } from '../../../services/handover-readiness.service';
import {
  HandoverReadinessItemDto,
  HandoverReadinessStatus,
  AddHandoverItemDialogResult,
  BlockHandoverItemDialogData,
  BlockHandoverItemDialogResult,
} from '../../../models/handover-readiness.model';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { DialogService } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { StatusSelectComponent, StatusSelectOption } from '@common/components/form/status-select/status-select';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { AlertComponent } from '@common/components/alert/alert';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AddHandoverItemDialogComponent } from './dialogs/add-handover-item-dialog/add-handover-item-dialog.component';
import { BlockHandoverItemDialogComponent } from './dialogs/block-handover-item-dialog/block-handover-item-dialog.component';
import { DocumentService } from '../../../services/document.service';
import { USER_ROLES } from '@shared/constants/users.constants';
import {
  AttachEvidenceDialogComponent,
  AttachEvidenceDialogData,
  AttachEvidenceDialogResult
} from '../../checklist/dialogs/attach-evidence-dialog/attach-evidence-dialog.component';

import {
  HANDOVER_READINESS_LABELS,
  HANDOVER_READINESS_MESSAGES
} from '@shared/constants/handover.constants';

@Component({
  selector: 'app-handover-readiness',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonComponent,
    StatusBadgeComponent,
    StatusSelectComponent,
    LoadingSpinnerComponent,
    ConfirmDialogComponent,
    AlertComponent,
    InlineSVGModule,
  ],
  templateUrl: './handover-readiness.component.html',
  styleUrl: './handover-readiness.component.scss'
})
export class HandoverReadinessComponent implements OnInit {
  private readinessService = inject(HandoverReadinessService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);
  private documentService = inject(DocumentService);

  readonly LABELS = HANDOVER_READINESS_LABELS;
  readonly MESSAGES = HANDOVER_READINESS_MESSAGES;

  readonly engagementId = input.required<number>();
  readonly isSubmitted = input<boolean>(false);
  readonly itemsUpdated = output<HandoverReadinessItemDto[]>();

  items = signal<HandoverReadinessItemDto[]>([]);
  isLoading = signal<boolean>(false);
  pendingDeleteItem = signal<HandoverReadinessItemDto | null>(null);
  isDeletingItem = signal<boolean>(false);
  errorMessage = signal<string>('');

  readonly isPreSales = computed(() => this.authService.hasRole(USER_ROLES.PRE_SALES));

  readonly blockedCount = computed(() =>
    this.items().filter(item => item.status === 'Blocked').length
  );

  readonly commercialItems = computed(() =>
    this.items().filter(item => item.area.toUpperCase() === 'COMMERCIAL')
  );

  readonly handoverItems = computed(() =>
    this.items().filter(item => item.area.toUpperCase() === 'HANDOVER')
  );

  readonly statusOptions: StatusSelectOption[] = [
    { label: 'In Progress', value: 'InProgress', tone: 'info' },
    { label: 'Granted', value: 'Granted', tone: 'success' },
    { label: 'Blocked', value: 'Blocked', tone: 'danger' }
  ];

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(skipLoader = false, onDone?: () => void): void {
    if (skipLoader) {
      this.isLoading.set(true);
    }
    this.readinessService.getItems(this.engagementId(), skipLoader).subscribe({
      next: (data) => {
        this.items.set(data);
        if (skipLoader) {
          this.isLoading.set(false);
        }
        this.itemsUpdated.emit(data);
        onDone?.();
      },
      error: () => {
        if (skipLoader) {
          this.isLoading.set(false);
        }
        this.toastService.error(this.MESSAGES.LOAD_ERROR);
        onDone?.();
      }
    });
  }

  onStatusChange(item: HandoverReadinessItemDto, newStatus: HandoverReadinessStatus): void {
    if (!this.isPreSales()) return;

    if (newStatus === item.status) return;

    if (newStatus === 'Blocked') {
      const dialogRef = this.dialogService.open<BlockHandoverItemDialogComponent, BlockHandoverItemDialogData, BlockHandoverItemDialogResult>(
        BlockHandoverItemDialogComponent,
        {
          data: {
            itemName: item.name,
            currentReason: item.blockReason || ''
          }
        }
      );

      dialogRef.afterClosed().subscribe((result: BlockHandoverItemDialogResult | undefined) => {
        if (result && result.reason) {
          this.updateStatus(item, 'Blocked', result.reason);
        } else {
          this.items.update(current =>
            current.map(i => i.id === item.id ? { ...i } : i)
          );
        }
      });
    } else {
      this.updateStatus(item, newStatus, null);
    }
  }

  private updateStatus(item: HandoverReadinessItemDto, status: HandoverReadinessStatus, blockReason: string | null): void {
    this.readinessService.updateStatus(item.id, { status, blockReason: blockReason || undefined }).subscribe({
      next: (updatedItem) => {
        this.items.update(current =>
          current.map(i => i.id === updatedItem.id ? updatedItem : i)
        );
        this.toastService.success(this.MESSAGES.STATUS_UPDATED);
        this.itemsUpdated.emit(this.items());
      },
      error: (err) => {
        this.loadItems(true, () =>
          this.toastService.error(err?.error?.message || this.MESSAGES.STATUS_UPDATE_ERROR)
        );
      }
    });
  }

  openAddModal(area: 'COMMERCIAL' | 'HANDOVER' = 'HANDOVER'): void {
    if (!this.isPreSales()) return;

    const dialogRef = this.dialogService.open<AddHandoverItemDialogComponent, void, AddHandoverItemDialogResult>(
      AddHandoverItemDialogComponent
    );

    dialogRef.afterClosed().subscribe((result: AddHandoverItemDialogResult | undefined) => {
      if (result && result.name) {
        this.readinessService.addCustomItem(this.engagementId(), { name: result.name, area }).subscribe({
          next: (newItem) => {
            this.items.update(current => [...current, newItem]);
            this.toastService.success(this.MESSAGES.ADD_SUCCESS(area));
            this.itemsUpdated.emit(this.items());
          },
          error: (err) => {
            this.toastService.error(err?.error?.message || this.MESSAGES.ADD_ERROR);
          }
        });
      }
    });
  }

  openAttachEvidenceDialog(item: HandoverReadinessItemDto): void {
    const dialogRef = this.dialogService.open<AttachEvidenceDialogComponent, AttachEvidenceDialogData, AttachEvidenceDialogResult>(
      AttachEvidenceDialogComponent,
      {
        data: {
          item: {
            id: item.id,
            title: item.name,
            evidenceDocumentId: item.evidenceDocumentId,
            evidenceDocumentFileName: item.evidenceDocumentFileName,
            evidenceSubmittedByName: item.evidenceSubmittedByName,
            evidenceSubmittedAt: item.evidenceSubmittedAt
          } as any,
          engagementId: this.engagementId(),
          isReadOnly: !this.isPreSales() || this.isSubmitted(),
          saveEvidence: (documentId: number) =>
            this.readinessService.saveEvidence(this.engagementId(), item.id, documentId, true)
        }
      }
    );

    dialogRef.afterClosed().subscribe((result: AttachEvidenceDialogResult | undefined) => {
      if (!result) return;

      this.loadItems(true, () => this.toastService.success(this.MESSAGES.EVIDENCE_SAVE_SUCCESS));
    });
  }

  downloadEvidence(item: HandoverReadinessItemDto): void {
    if (!item.evidenceDocumentId) return;
    this.documentService.downloadDocument(item.evidenceDocumentId, item.evidenceDocumentFileName || 'evidence').subscribe();
  }

  onDeleteItem(item: HandoverReadinessItemDto): void {
    if (!this.isPreSales()) return;
    this.errorMessage.set('');
    this.pendingDeleteItem.set(item);
  }

  cancelDeleteItem(): void {
    this.pendingDeleteItem.set(null);
    this.errorMessage.set('');
  }

  confirmDeleteItem(item: HandoverReadinessItemDto): void {
    this.errorMessage.set('');
    this.isDeletingItem.set(true);
    this.readinessService.deleteItem(item.id, true).subscribe({
      next: () => {
        this.pendingDeleteItem.set(null);
        this.isDeletingItem.set(false);
        this.loadItems(true, () =>
          this.toastService.success(this.MESSAGES.DELETE_SUCCESS)
        );
      },
      error: (err) => {
        this.isDeletingItem.set(false);
        this.errorMessage.set(err?.error?.message || this.MESSAGES.DELETE_ERROR);
      }
    });
  }

  getStatusTone(status: string): StatusBadgeTone {
    switch (status) {
      case 'Granted':
        return 'success';
      case 'Blocked':
        return 'danger';
      case 'InProgress':
      default:
        return 'warning';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'InProgress':
        return 'In Progress';
      case 'Granted':
        return 'Granted';
      case 'Blocked':
        return 'Blocked';
      default:
        return status;
    }
  }
}
