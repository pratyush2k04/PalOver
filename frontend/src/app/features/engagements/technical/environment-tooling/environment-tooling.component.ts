import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TECHNICAL_READINESS_ROLES } from '../../../../constants/app.constants';
import { EnvironmentToolingService } from '../../services/environment-tooling.service';
import { AuthService } from 'src/app/services/auth';
import {
  EnvironmentToolingItemDto,
  EnvironmentToolingStatus
} from '../../models/environment-tooling.model';

import { InlineSVGModule } from 'ng-inline-svg-2';
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
  imports: [CommonModule, FormsModule, InlineSVGModule, StatusBadgeComponent, ButtonComponent],
  templateUrl: './environment-tooling.component.html',
  styleUrl: './environment-tooling.component.scss'
})
export class EnvironmentToolingComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<EnvironmentToolingItemDto[]>();

  private service = inject(EnvironmentToolingService);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  private dialog = inject(DialogService);

  items = signal<EnvironmentToolingItemDto[]>([]);
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));

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
        const list = res && res.items && res.items.length > 0 ? res.items : this.getDummyItems();
        this.items.set(list);
        this.itemsUpdated.emit(list);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.warn('API load environment tooling failed, falling back to dummy items', err);
        const dummy = this.getDummyItems();
        this.items.set(dummy);
        this.itemsUpdated.emit(dummy);
        this.isLoading.set(false);
      }
    });
  }

  private getDummyItems(): EnvironmentToolingItemDto[] {
    return [
      {
        environmentToolingItemId: 201,
        name: 'Dev environment provisioned',
        engagementId: this.engagementId(),
        ownerId: 1,
        ownerFullName: 'Ananth Vishwanatha',
        status: EnvironmentToolingStatus.Ready
      },
      {
        environmentToolingItemId: 202,
        name: 'CI/CD pipeline access',
        engagementId: this.engagementId(),
        ownerId: 1,
        ownerFullName: 'Ananth Vishwanatha',
        status: EnvironmentToolingStatus.NotReady
      },
      {
        environmentToolingItemId: 203,
        name: 'Shared repository created',
        engagementId: this.engagementId(),
        ownerId: 1,
        ownerFullName: 'Ananth Vishwanatha',
        status: EnvironmentToolingStatus.Ready
      }
    ];
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
      { data: { itemName: item.name, currentReason: item.notApplicableReason } }
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

    this.service.updateStatus(item.environmentToolingItemId, payload).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (updatedItem) => {
        const updatedList = this.items().map(i =>
          i.environmentToolingItemId === updatedItem.environmentToolingItemId ? updatedItem : i
        );
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      },
      error: (err) => {
        console.warn('API update environment status failed, updating local state for testing', err);
        const updatedList = this.items().map(i => {
          if (i.environmentToolingItemId === item.environmentToolingItemId) {
            return {
              ...i,
              status: newStatus,
              notApplicableReason: newStatus === EnvironmentToolingStatus.NotApplicable ? naReason : undefined
            };
          }
          return i;
        });
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
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
      AddEnvironmentItemDialogComponent
    );

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;
      this.submitAddItem(result);
    });
  }

  private submitAddItem(result: AddEnvironmentItemDialogResult) {
    const requestData = {
      customName: result.name,
      ownerId: 1 // Default to Rahul Verma (ID 1)
    };

    this.service.addCustomItem(this.engagementId(), requestData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (newItem) => {
        const updatedList = [...this.items(), newItem];
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      },
      error: (err) => {
        console.warn('API add environment item failed, adding local item for testing', err);
        const localNewItem: EnvironmentToolingItemDto = {
          environmentToolingItemId: Date.now(),
          name: requestData.customName,
          customName: requestData.customName,
          engagementId: this.engagementId(),
          ownerId: 1,
          ownerFullName: 'Rahul Verma',
          status: EnvironmentToolingStatus.NotReady
        };
        const updatedList = [...this.items(), localNewItem];
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      }
    });
  }
}
