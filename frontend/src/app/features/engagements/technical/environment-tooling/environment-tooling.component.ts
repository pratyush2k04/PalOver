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

@Component({
  selector: 'app-environment-tooling',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule],
  templateUrl: './environment-tooling.component.html',
  styleUrl: './environment-tooling.component.scss'
})
export class EnvironmentToolingComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<EnvironmentToolingItemDto[]>();

  private service = inject(EnvironmentToolingService);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);

  items = signal<EnvironmentToolingItemDto[]>([]);
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));

  // Add Item Modal
  isAddModalOpen = signal<boolean>(false);
  newItemName = signal<string>('');
  nameError = signal<string>('');

  // Not Applicable Reason Modal
  isNaModalOpen = signal<boolean>(false);
  naReasonText = signal<string>('');
  naReasonError = signal<string>('');
  itemForNa = signal<EnvironmentToolingItemDto | null>(null);

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
      this.itemForNa.set(item);
      this.naReasonText.set(item.notApplicableReason || '');
      this.naReasonError.set('');
      this.isNaModalOpen.set(true);
      return;
    }

    this.updateStatus(item, newStatus);
  }

  closeNaModal() {
    this.isNaModalOpen.set(false);
    this.itemForNa.set(null);
    this.naReasonText.set('');
    this.naReasonError.set('');
  }

  submitNaReason() {
    const item = this.itemForNa();
    if (!item) return;

    const reason = this.naReasonText().trim();
    if (!reason) {
      this.naReasonError.set('Reason is required for Not Applicable status');
      return;
    }

    this.naReasonError.set('');
    this.updateStatus(item, EnvironmentToolingStatus.NotApplicable, reason);
    this.closeNaModal();
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

  getStatusBadgeClass(status: EnvironmentToolingStatus | number | string): string {
    const val = typeof status === 'string' ? parseInt(status, 10) : status;
    switch (val) {
      case EnvironmentToolingStatus.Ready:
      case 2:
        return 'badge-ready';
      case EnvironmentToolingStatus.NotApplicable:
      case 3:
        return 'badge-na';
      default:
        return 'badge-inprogress';
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

  openAddModal() {
    this.newItemName.set('');
    this.nameError.set('');
    this.isAddModalOpen.set(true);
  }

  closeAddModal() {
    this.nameError.set('');
    this.isAddModalOpen.set(false);
  }

  submitAddItem() {
    if (!this.newItemName().trim()) {
      this.nameError.set('This field is required');
      return;
    }

    this.nameError.set('');

    const requestData = {
      customName: this.newItemName().trim(),
      ownerId: 1 // Default to Rahul Verma (ID 1)
    };

    this.service.addCustomItem(this.engagementId(), requestData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (newItem) => {
        const updatedList = [...this.items(), newItem];
        this.items.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.closeAddModal();
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
        this.closeAddModal();
      }
    });
  }
}
