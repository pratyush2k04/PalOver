import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
  isTechLead = computed(() => this.engagementService.isWorkspaceTechLead());
  canManage = computed(() => this.isTechLead() && !this.engagementService.isSignedOff());
  itemToDelete = signal<EnvironmentToolingItemDto | null>(null);
  isDeleting = signal<boolean>(false);

  private static readonly ALLOWED_TRANSITIONS = new Set<string>([
    `${EnvironmentToolingStatus.InProgress}->${EnvironmentToolingStatus.Granted}`,
    `${EnvironmentToolingStatus.InProgress}->${EnvironmentToolingStatus.Blocked}`,
    `${EnvironmentToolingStatus.Granted}->${EnvironmentToolingStatus.Blocked}`,
    `${EnvironmentToolingStatus.Blocked}->${EnvironmentToolingStatus.InProgress}`,
    `${EnvironmentToolingStatus.Blocked}->${EnvironmentToolingStatus.Granted}`,
  ]);

  getStatusNumericValue(status: EnvironmentToolingStatus | string | number | unknown): EnvironmentToolingStatus {
    if (status === EnvironmentToolingStatus.Granted || status === 'Granted' || Number(status) === EnvironmentToolingStatus.Granted) {
      return EnvironmentToolingStatus.Granted;
    }
    if (status === EnvironmentToolingStatus.Blocked || status === 'Blocked' || Number(status) === EnvironmentToolingStatus.Blocked) {
      return EnvironmentToolingStatus.Blocked;
    }
    return EnvironmentToolingStatus.InProgress;
  }

  getStatusOptions(status: EnvironmentToolingStatus | number | string): StatusSelectOption[] {
    const current = this.getStatusNumericValue(status);
    return [
      {
        label: 'In Progress',
        value: EnvironmentToolingStatus.InProgress,
        tone: 'info',
        disabled: current !== EnvironmentToolingStatus.InProgress && !this.isTransitionAllowed(current, EnvironmentToolingStatus.InProgress)
      },
      {
        label: 'Granted',
        value: EnvironmentToolingStatus.Granted,
        tone: 'success',
        disabled: current !== EnvironmentToolingStatus.Granted && !this.isTransitionAllowed(current, EnvironmentToolingStatus.Granted)
      },
      {
        label: 'Blocked',
        value: EnvironmentToolingStatus.Blocked,
        tone: 'danger',
        disabled: current !== EnvironmentToolingStatus.Blocked && !this.isTransitionAllowed(current, EnvironmentToolingStatus.Blocked)
      }
    ];
  }

  isTransitionAllowed(from: EnvironmentToolingStatus, to: EnvironmentToolingStatus): boolean {
    if (from === to) return false;
    return EnvironmentToolingComponent.ALLOWED_TRANSITIONS.has(`${from}->${to}`);
  }

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadItems(id);
      }
    });
  }

  loadItems(id: number) {
    this.service.getItems(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res) => {
        const list = res?.items ?? [];
        this.items.set(list);
        this.itemsUpdated.emit(list);
      },
      error: (err) => {
        console.error('API load environment tooling failed', err);
        this.items.set([]);
        this.itemsUpdated.emit([]);
      }
    });
  }

  onStatusChange(item: EnvironmentToolingItemDto, newStatus: unknown): void {
    const currentStatus = this.getStatusNumericValue(item.status);
    const statusVal = this.getStatusNumericValue(newStatus);

    if (currentStatus === statusVal) {
      return;
    }

    if (!this.isTransitionAllowed(currentStatus, statusVal)) {
      this.toastService.error(`Cannot change status from ${this.getStatusLabel(currentStatus)} to ${this.getStatusLabel(statusVal)} for ${item.name}`);
      return;
    }

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
      error: () => {
        this.toastService.error(`Failed to update status for ${item.name}`);
      }
    });
  }

  getStatusTone(status: EnvironmentToolingStatus | number | string): StatusBadgeTone {
    const val = typeof status === 'string' ? parseInt(status, 10) : status;
    switch (val) {
      case EnvironmentToolingStatus.Granted:
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
      case EnvironmentToolingStatus.Granted:
      case 2:
        return 'Granted';
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
        console.error('API add environment tooling item failed', err);
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
    this.service.deleteItem(item.id, true).pipe(
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
