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
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { AddHandoverItemDialogComponent } from './dialogs/add-handover-item-dialog/add-handover-item-dialog.component';
import { BlockHandoverItemDialogComponent } from './dialogs/block-handover-item-dialog/block-handover-item-dialog.component';

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
    ConfirmDialogComponent
  ],
  templateUrl: './handover-readiness.component.html',
  styleUrl: './handover-readiness.component.scss'
})
export class HandoverReadinessComponent implements OnInit {
  private readinessService = inject(HandoverReadinessService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);

  readonly engagementId = input.required<number>();
  readonly isSubmitted = input<boolean>(false);
  readonly itemsUpdated = output<HandoverReadinessItemDto[]>();

  items = signal<HandoverReadinessItemDto[]>([]);
  isLoading = signal<boolean>(true);
  pendingDeleteItem = signal<HandoverReadinessItemDto | null>(null);
  isDeletingItem = signal<boolean>(false);

  readonly isPreSales = computed(() => this.authService.hasRole('Pre Sales'));

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

  loadItems(): void {
    this.isLoading.set(true);
    this.readinessService.getItems(this.engagementId()).subscribe({
      next: (data) => {
        this.items.set(data);
        this.isLoading.set(false);
        this.itemsUpdated.emit(data);
      },
      error: () => {
        this.toastService.error('Failed to load Handover Readiness items');
        this.isLoading.set(false);
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
        this.toastService.success('Status updated');
        this.itemsUpdated.emit(this.items());
      },
      error: (err) => {
        this.toastService.error(err?.error?.message || 'Failed to update status');
        this.loadItems();
      }
    });
  }

  openAddModal(): void {
    if (!this.isPreSales()) return;

    const dialogRef = this.dialogService.open<AddHandoverItemDialogComponent, void, AddHandoverItemDialogResult>(
      AddHandoverItemDialogComponent
    );

    dialogRef.afterClosed().subscribe((result: AddHandoverItemDialogResult | undefined) => {
      if (result && result.name) {
        this.readinessService.addCustomItem(this.engagementId(), { name: result.name }).subscribe({
          next: (newItem) => {
            this.items.update(current => [...current, newItem]);
            this.toastService.success('Custom handover item added successfully');
            this.itemsUpdated.emit(this.items());
          },
          error: (err) => {
            this.toastService.error(err?.error?.message || 'Failed to add custom item');
          }
        });
      }
    });
  }

  onDeleteItem(item: HandoverReadinessItemDto): void {
    if (!this.isPreSales()) return;
    this.pendingDeleteItem.set(item);
  }

  cancelDeleteItem(): void {
    this.pendingDeleteItem.set(null);
  }

  confirmDeleteItem(item: HandoverReadinessItemDto): void {
    this.isDeletingItem.set(true);
    this.readinessService.deleteItem(item.id).subscribe({
      next: () => {
        this.items.update(current => current.filter(i => i.id !== item.id));
        this.toastService.success('Item deleted successfully');
        this.itemsUpdated.emit(this.items());
        this.pendingDeleteItem.set(null);
        this.isDeletingItem.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message || 'Failed to delete item');
        this.isDeletingItem.set(false);
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
