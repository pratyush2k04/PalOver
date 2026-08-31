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
import { ButtonComponent } from '@common/components/button/button';
import { DialogService } from '@common/services/dialog/dialog.service';
import {
  AddEnvironmentItemDialogComponent,
  AddEnvironmentItemDialogResult
} from './dialogs/add-environment-item-dialog/add-environment-item-dialog.component';
import {
  NotApplicableReasonDialogComponent,
  NotApplicableReasonDialogData,
  NotApplicableReasonDialogResult
} from './dialogs/not-applicable-reason-dialog/not-applicable-reason-dialog.component';

@Component({
  selector: 'app-environment-tooling',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    CommonModule,
    FormsModule,
    InlineSVGModule,
    StatusBadgeComponent,
    ButtonComponent
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

  readonly statusOptions = [
    { label: 'Ready', value: EnvironmentToolingStatus.Ready },
    { label: 'In Progress', value: EnvironmentToolingStatus.NotReady },
    { label: 'Not Applicable', value: EnvironmentToolingStatus.NotApplicable }
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

  onStatusChange(item: EnvironmentToolingItemDto, event: Event) {
    const select = event.target as HTMLSelectElement;
    const newStatus = parseInt(select.value, 10) as EnvironmentToolingStatus;

    if (newStatus === EnvironmentToolingStatus.NotApplicable) {
      select.value = item.status.toString();
      this.openNaDialog(item);
      return;
    }

    this.updateStatus(item, newStatus);
  }

  private openNaDialog(item: EnvironmentToolingItemDto): void {
    const dialogRef = this.dialog.open<NotApplicableReasonDialogComponent, NotApplicableReasonDialogData, NotApplicableReasonDialogResult>(
      NotApplicableReasonDialogComponent,
      { data: { itemName: item.name, currentReason: item.notApplicableReason }, disableClose: true }
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.updateStatus(item, EnvironmentToolingStatus.NotApplicable, result.reason);
    });
  }

  private updateStatus(item: EnvironmentToolingItemDto, newStatus: EnvironmentToolingStatus, naReason?: string) {
    const payload = {
      status: newStatus,
      notApplicableReason: newStatus === EnvironmentToolingStatus.NotApplicable ? naReason : undefined
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
      }
    });
  }

  getStatusTone(status: EnvironmentToolingStatus | number | string): StatusBadgeTone {
    const val = typeof status === 'string' ? parseInt(status, 10) : status;
    switch (val) {
      case EnvironmentToolingStatus.Ready:
      case 2:
        return 'success';
      case EnvironmentToolingStatus.NotApplicable:
      case 3:
        return 'neutral';
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
      case EnvironmentToolingStatus.NotApplicable:
      case 3:
        return 'Not Applicable';
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
}
