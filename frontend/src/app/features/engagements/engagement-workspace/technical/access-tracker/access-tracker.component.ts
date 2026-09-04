import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/app.constants';
import { TechnicalReadinessService } from '@features/engagements/services/technical-readiness.service';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { AuthService } from '@shared/services/auth';
import { AccessItemDto } from '@features/engagements/models/access-tracker/access-tracker.dtos';
import { AccessItemStatus } from '@features/engagements/models/access-tracker/access-tracker.enums';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { StatusSelectComponent, StatusSelectOption } from '@common/components/form/status-select/status-select';
import { ButtonComponent } from '@common/components/button/button';
import { ConfirmDialogComponent } from '@common/components/confirm-dialog/confirm-dialog';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  AddAccessItemDialogComponent,
  AddAccessItemDialogResult
} from './dialogs/add-access-item-dialog/add-access-item-dialog.component';
import {
  BlockAccessItemDialogComponent,
  BlockAccessItemDialogData,
  BlockAccessItemDialogResult
} from './dialogs/block-access-item-dialog/block-access-item-dialog.component';


@Component({
  selector: 'app-access-tracker',
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
  templateUrl: './access-tracker.component.html',
  styleUrl: './access-tracker.component.scss'
})
export class AccessTrackerComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<AccessItemDto[]>();

  private readinessService = inject(TechnicalReadinessService);
  private toastService = inject(ToastService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private authService = inject(AuthService);
  private engagementService = inject(EngagementService);
  private destroyRef = inject(DestroyRef);
  private dialog = inject(DialogService);

  accessItems = signal<AccessItemDto[]>([]);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  canManage = computed(() => this.isTechLead() && !this.engagementService.isSignedOff());

  itemToDelete = signal<AccessItemDto | null>(null);
  isDeleting = signal<boolean>(false);

  kickoffReadiness = this.kickoffReadinessService.readiness;

  readonly statusOptions: StatusSelectOption[] = [
    { label: 'In Progress', value: AccessItemStatus.InProgress, tone: 'info' },
    { label: 'Granted', value: AccessItemStatus.Granted, tone: 'success' },
    { label: 'Blocked', value: AccessItemStatus.Blocked, tone: 'danger' }
  ];

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadItems(id);
        this.kickoffReadinessService.track(id);
      }
    });
  }

  loadItems(id: number) {
    this.readinessService.getAccessItems(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (items) => {
        const finalItems = items ?? [];
        this.accessItems.set(finalItems);
        this.itemsUpdated.emit(finalItems);
      },
      error: (err) => {
        console.error('Failed to load access items from API', err);
        this.accessItems.set([]);
        this.itemsUpdated.emit([]);
      }
    });
  }

  onStatusChange(item: AccessItemDto, newStatus: unknown): void {
    const newStatusNum = this.getStatusNumericValue(newStatus as AccessItemStatus);

    if (newStatusNum === AccessItemStatus.Blocked) {
      this.openBlockDialog(item);
      return;
    }

    this.updateStatus(item, newStatusNum);
  }

  private openBlockDialog(item: AccessItemDto): void {
    const dialogRef = this.dialog.open<BlockAccessItemDialogComponent, BlockAccessItemDialogData, BlockAccessItemDialogResult>(
      BlockAccessItemDialogComponent,
      { data: { itemName: item.name, currentReason: item.blockReason }, disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) {
        // Force signal update so ngModel re-syncs to original status
        this.accessItems.update((items) => items.map((i) => i.id === item.id ? { ...i } : i));
        return;
      }
      this.updateStatus(item, AccessItemStatus.Blocked, result.reason);
    });
  }

  private updateStatus(item: AccessItemDto, newStatusNum: AccessItemStatus, blockReason?: string): void {
    const payload = {
      status: newStatusNum,
      blockReason: newStatusNum === AccessItemStatus.Blocked ? blockReason : undefined
    };

    this.readinessService.updateAccessItemStatus(item.id, payload).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (updatedItem) => {
        const updatedList = this.accessItems().map(i =>
          i.id === updatedItem.id ? updatedItem : i
        );
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success('Status updated');
      },
      error: (err) => {
        this.toastService.error(`Failed to update status for ${item.name}`);
        // Revert UI on error
        this.accessItems.update((items) => items.map((i) => i.id === item.id ? { ...i } : i));
      }
    });
  }

  getStatusNumericValue(status: AccessItemStatus | string | number): AccessItemStatus {
    if (status === AccessItemStatus.Granted || status === 'Granted' || +status === AccessItemStatus.Granted) {
      return AccessItemStatus.Granted;
    }
    if (status === AccessItemStatus.Blocked || status === 'Blocked' || +status === AccessItemStatus.Blocked) {
      return AccessItemStatus.Blocked;
    }
    return AccessItemStatus.InProgress;
  }

  getStatusLabel(status: AccessItemStatus | string | number): string {
    const num = this.getStatusNumericValue(status);
    switch (num) {
      case AccessItemStatus.Granted:
        return 'Granted';
      case AccessItemStatus.Blocked:
        return 'Blocked';
      case AccessItemStatus.InProgress:
      default:
        return 'In Progress';
    }
  }

  getStatusTone(status: AccessItemStatus | string | number): StatusBadgeTone {
    const num = this.getStatusNumericValue(status);
    switch (num) {
      case AccessItemStatus.Granted:
        return 'success';
      case AccessItemStatus.Blocked:
        return 'danger';
      case AccessItemStatus.InProgress:
      default:
        return 'info';
    }
  }

  getStatusBadgeClass(status: AccessItemStatus | string | number): string {
    const num = this.getStatusNumericValue(status);
    switch (num) {
      case AccessItemStatus.Granted:
        return 'badge-granted';
      case AccessItemStatus.Blocked:
        return 'badge-blocked';
      case AccessItemStatus.InProgress:
      default:
        return 'badge-inprogress';
    }
  }

  openAddModal(): void {
    const dialogRef = this.dialog.open<AddAccessItemDialogComponent, undefined, AddAccessItemDialogResult>(
      AddAccessItemDialogComponent,
      { disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.submitAddItem(result);
    });
  }



  private submitAddItem(result: AddAccessItemDialogResult) {
    const currentUserId = this.authService.getUserId() ?? 1;
    const currentUser = this.authService.currentUser();
    const requestData = {
      name: result.name,
      isClientSide: result.isClientSide,
      timeZoneNote: result.timeZoneNote,
      ownerId: currentUserId
    };

    this.readinessService.addCustomAccessItem(this.engagementId(), requestData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (newItem) => {
        const updatedList = [...this.accessItems(), newItem];
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success(`Access item "${newItem.name}" added successfully`);
      },
      error: (err) => {
        console.error('API add access item failed', err);
      }
    });
  }

  onDeleteItem(item: AccessItemDto): void {
    if (!this.isTechLead()) return;
    this.itemToDelete.set(item);
  }

  cancelDelete(): void {
    this.itemToDelete.set(null);
  }

  confirmDelete(): void {
    const item = this.itemToDelete();
    if (!item) return;

    this.isDeleting.set(true);
    this.readinessService.deleteAccessItem(item.id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        const updatedList = this.accessItems().filter(i => i.id !== item.id);
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success(`Access item "${item.name}" deleted successfully`);
        this.isDeleting.set(false);
        this.itemToDelete.set(null);
      },
      error: (err) => {
        this.toastService.error(`Failed to delete "${item.name}"`);
        console.error('API delete access item failed', err);
        this.isDeleting.set(false);
      }
    });
  }
}
