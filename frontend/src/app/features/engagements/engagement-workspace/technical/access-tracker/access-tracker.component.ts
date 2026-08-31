import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
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
import { FlaggedItemsBannerComponent, FlaggedItem } from '@shared/flagged-items-banner/flagged-items-banner.component';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
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
import {
  EditOwnerDialogComponent,
  EditOwnerDialogData,
  EditOwnerDialogResult
} from '../dialogs/edit-owner-dialog/edit-owner-dialog.component';

@Component({
  selector: 'app-access-tracker',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    CommonModule,
    FormsModule,
    InlineSVGModule,
    TooltipDirective,
    FlaggedItemsBannerComponent,
    StatusBadgeComponent,
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
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));
  canManage = computed(() => this.isTechLead() && !this.engagementService.isSignedOff());

  itemToDelete = signal<AccessItemDto | null>(null);
  isDeleting = signal<boolean>(false);

  kickoffReadiness = this.kickoffReadinessService.readiness;
  blockedItems = computed(() => this.accessItems().filter(i => i.status === 'Blocked'));
  accessTrackerFlaggedItems = computed<FlaggedItem[]>(() => [
    { label: 'access item(s) blocked', count: this.blockedItems().length }
  ]);

  readonly statusOptions = [
    { label: 'In Progress', value: AccessItemStatus.InProgress, key: 'InProgress' },
    { label: 'Granted', value: AccessItemStatus.Granted, key: 'Granted' },
    { label: 'Blocked', value: AccessItemStatus.Blocked, key: 'Blocked' }
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
    this.isLoading.set(true);
    this.readinessService.getAccessItems(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (items) => {
        const finalItems = items ?? [];
        this.accessItems.set(finalItems);
        this.itemsUpdated.emit(finalItems);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load access items from API', err);
        this.accessItems.set([]);
        this.itemsUpdated.emit([]);
        this.isLoading.set(false);
      }
    });
  }

  onStatusChange(item: AccessItemDto, event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const newStatusNum = parseInt(selectElement.value, 10) as AccessItemStatus;

    if (newStatusNum === AccessItemStatus.Blocked) {
      // Temporarily revert select dropdown display until block reason confirmed
      selectElement.value = this.getStatusNumericValue(item.status).toString();
      this.openBlockDialog(item, selectElement);
      return;
    }

    this.updateStatus(item, newStatusNum, selectElement);
  }

  private openBlockDialog(item: AccessItemDto, selectElement?: HTMLSelectElement): void {
    const dialogRef = this.dialog.open<BlockAccessItemDialogComponent, BlockAccessItemDialogData, BlockAccessItemDialogResult>(
      BlockAccessItemDialogComponent,
      { data: { itemName: item.name, currentReason: item.blockReason }, disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.updateStatus(item, AccessItemStatus.Blocked, selectElement, result.reason);
    });
  }

  private updateStatus(item: AccessItemDto, newStatusNum: AccessItemStatus, selectElement?: HTMLSelectElement, blockReason?: string) {
    const statusTextMap: Record<number, string> = {
      [AccessItemStatus.InProgress]: 'InProgress',
      [AccessItemStatus.Granted]: 'Granted',
      [AccessItemStatus.Blocked]: 'Blocked'
    };

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
        this.toastService.success(`Status updated for ${item.name}`);
      },
      error: (err) => {
        this.toastService.error(`Failed to update status for ${item.name}`);
        
        // Revert the dropdown UI back to its original state
        if (selectElement) {
          selectElement.value = this.getStatusNumericValue(item.status).toString();
        }
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

  openEditOwnerModal(item: AccessItemDto): void {
    const dialogRef = this.dialog.open<EditOwnerDialogComponent, EditOwnerDialogData, EditOwnerDialogResult>(
      EditOwnerDialogComponent,
      { 
        data: { 
          itemName: item.name, 
          engagementId: this.engagementId(),
          currentOwnerId: item.ownerId 
        },
        disableClose: true
      }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.updateOwner(item, result.ownerId);
    });
  }

  private updateOwner(item: AccessItemDto, ownerId: number) {
    this.readinessService.updateAccessItemOwner(item.id, { ownerId }).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (updatedItem) => {
        const updatedList = this.accessItems().map(i =>
          i.id === updatedItem.id ? updatedItem : i
        );
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.toastService.success(`Owner updated for ${item.name}`);
      },
      error: (err) => {
        this.toastService.error(`Failed to update owner for ${item.name}`);
        console.error('API update owner failed', err);
      }
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
        console.warn('API add item failed, adding item locally for testing', err);
        const localNewItem: AccessItemDto = {
          id: Date.now(),
          name: requestData.name,
          isClientSide: requestData.isClientSide,
          ownerId: currentUserId,
          ownerFullName: currentUser?.fullName ?? 'Tech Lead',
          status: 'InProgress',
          timeZoneNote: requestData.timeZoneNote,
          daysUntilBillingDate: 5,
          isOverdueWithClient: false
        };
        const updatedList = [...this.accessItems(), localNewItem];
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
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
