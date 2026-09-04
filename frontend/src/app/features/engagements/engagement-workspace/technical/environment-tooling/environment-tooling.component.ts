import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/app.constants';
import { EnvironmentToolingService } from '@features/engagements/services/environment-tooling.service';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { AuthService } from '@shared/services/auth';
import { EnvironmentToolingItemDto, EnvironmentToolingReadinessResponse } from '@features/engagements/models/environment-tooling/environment-tooling.dtos';
import { EnvironmentToolingStatus } from '@features/engagements/models/environment-tooling/environment-tooling.enums';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { StatusSelectComponent, StatusSelectOption } from '@common/components/form/status-select/status-select';
import { ButtonComponent } from '@common/components/button/button';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  AddEnvironmentItemDialogComponent,
  AddEnvironmentItemDialogResult
} from './dialogs/add-environment-item-dialog/add-environment-item-dialog.component';
import {
  BlockEnvironmentItemDialogComponent,
  BlockEnvironmentItemDialogData,
  BlockEnvironmentItemDialogResult
} from './dialogs/block-environment-item-dialog/block-environment-item-dialog.component';

@Component({
  selector: 'app-environment-tooling',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    CommonModule,
    FormsModule,
    InlineSVGModule,
    TooltipDirective,
    StatusBadgeComponent,
    StatusSelectComponent,
    ButtonComponent,
    ConfirmDialogComponent
  ],
  templateUrl: './environment-tooling.component.html',
  styleUrl: './environment-tooling.component.scss'
})
export class EnvironmentToolingComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<EnvironmentToolingItemDto[]>();

  private service = inject(EnvironmentToolingService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private destroyRef = inject(DestroyRef);
  private dialog = inject(DialogService);

  items = signal<EnvironmentToolingItemDto[]>([]);
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  canManage = computed(() => this.isTechLead() && !this.engagementService.isSignedOff());
  itemToDelete = signal<EnvironmentToolingItemDto | null>(null);
  isDeleting = signal<boolean>(false);

  readonly statusOptions: StatusSelectOption[] = [
    { label: 'Ready', value: EnvironmentToolingStatus.Ready, tone: 'success' },
    { label: 'In Progress', value: EnvironmentToolingStatus.NotReady, tone: 'info' },
    { label: 'Blocked', value: EnvironmentToolingStatus.Blocked, tone: 'danger' }
  ];

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadItems(id);
      }
    });
  }

  loadItems(id: number) {
    this.isLoading.set(true);
    this.service.getItems(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res) => {
        const list = res?.items ?? [];
        this.items.set(list);
        this.itemsUpdated.emit(list);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('API load environment tooling failed', err);
        this.items.set([]);
        this.itemsUpdated.emit([]);
        this.isLoading.set(false);
      }
    });
  }

  onStatusChange(item: EnvironmentToolingItemDto, newStatus: unknown): void {
    const statusVal = typeof newStatus === 'string' ? parseInt(newStatus, 10) : (newStatus as EnvironmentToolingStatus);

    if (statusVal === EnvironmentToolingStatus.Blocked) {
      this.openBlockDialog(item);
      return;
    }

    this.updateStatus(item, statusVal);
  }

  private openBlockDialog(item: EnvironmentToolingItemDto): void {
    const dialogRef = this.dialog.open<BlockEnvironmentItemDialogComponent, BlockEnvironmentItemDialogData, BlockEnvironmentItemDialogResult>(
      BlockEnvironmentItemDialogComponent,
      { data: { itemName: item.name, currentReason: item.blockReason }, disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) {
        // Revert UI on dialog cancel
        this.items.update((items) => items.map((i) => i.id === item.id ? { ...i } : i));
        return;
      }
      this.updateStatus(item, EnvironmentToolingStatus.Blocked, result.reason);
    });
  }

  private updateStatus(item: EnvironmentToolingItemDto, newStatus: EnvironmentToolingStatus, blockReason?: string): void {
    const payload = {
      status: newStatus,
      blockReason: newStatus === EnvironmentToolingStatus.Blocked ? blockReason : undefined
    };

    this.service.updateStatus(item.id, payload).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (updatedItem) => {
        const updatedList = this.items().map(i =>
          i.id === updatedItem.id ? updatedItem : i
        );
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success('Status updated');
      },
      error: (err) => {
        this.toastService.error(`Failed to update status for ${item.name}`);
        console.warn('API update environment status failed', err);
        // Revert UI on error
        this.items.update((items) => items.map((i) => i.id === item.id ? { ...i } : i));
      }
    });
  }

  getStatusTone(status: EnvironmentToolingStatus | number | string): StatusBadgeTone {
    const val = typeof status === 'string' ? parseInt(status, 10) : status;
    switch (val) {
      case EnvironmentToolingStatus.Ready:
      case 2:
        return 'success';
      case EnvironmentToolingStatus.Blocked:
      case 3:
        return 'danger';
      default:
        return 'info';
    }
  }

  getStatusLabel(status: EnvironmentToolingStatus | number | string): string {
    const val = typeof status === 'string' ? parseInt(status, 10) : status;
    switch (val) {
      case EnvironmentToolingStatus.Ready:
      case 2:
        return 'Ready';
      case EnvironmentToolingStatus.Blocked:
      case 3:
        return 'Blocked';
      default:
        return 'In Progress';
    }
  }

  openAddModal(): void {
    const dialogRef = this.dialog.open<AddEnvironmentItemDialogComponent, undefined, AddEnvironmentItemDialogResult>(
      AddEnvironmentItemDialogComponent,
      { disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.submitAddItem(result);
    });
  }



  private submitAddItem(result: AddEnvironmentItemDialogResult) {
    const currentUserId = this.authService.getUserId() ?? 1;
    const currentUser = this.authService.currentUser();
    const requestData = {
      customName: result.name,
      ownerId: currentUserId
    };

    this.service.addCustomItem(this.engagementId(), requestData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (newItem) => {
        const updatedList = [...this.items(), newItem];
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success(`Environment item "${newItem.name}" added successfully`);
      },
      error: (err) => {
        console.warn('API add environment item failed, adding local item for testing', err);
        const localNewItem: EnvironmentToolingItemDto = {
          id: Date.now(),
          name: requestData.customName,
          customName: requestData.customName,
          engagementId: this.engagementId(),
          ownerId: currentUserId,
          ownerFullName: currentUser?.fullName ?? 'Tech Lead',
          status: EnvironmentToolingStatus.NotReady
        };
        const updatedList = [...this.items(), localNewItem];
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      }
    });
  }

  onDeleteItem(item: EnvironmentToolingItemDto): void {
    if (!this.canManage()) return;
    this.itemToDelete.set(item);
  }

  cancelDelete(): void {
    this.itemToDelete.set(null);
  }

  confirmDelete(): void {
    const item = this.itemToDelete();
    if (!item) return;

    this.isDeleting.set(true);
    this.service.deleteItem(item.id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        const updatedList = this.items().filter(i => i.id !== item.id);
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success(`Environment item "${item.name}" deleted successfully`);
        this.isDeleting.set(false);
        this.itemToDelete.set(null);
      },
      error: (err) => {
        this.toastService.error(`Failed to delete "${item.name}"`);
        console.error('API delete environment item failed', err);
        this.isDeleting.set(false);
      }
    });
  }
}
