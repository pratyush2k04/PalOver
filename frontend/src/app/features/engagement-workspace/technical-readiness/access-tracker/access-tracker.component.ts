import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TechnicalReadinessService } from '../../../../services/technical-readiness.service';
import { AuthService } from '../../../../services/auth';
import { AccessItemDto, AccessItemStatus } from '../../../../models/technical-readiness.model';

@Component({
  selector: 'app-access-tracker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './access-tracker.component.html',
  styleUrl: './access-tracker.component.scss'
})
export class AccessTrackerComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<AccessItemDto[]>();

  private readinessService = inject(TechnicalReadinessService);
  private authService = inject(AuthService);

  accessItems = signal<AccessItemDto[]>([]);
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole('Tech Lead'));

  // Add Item Modal state
  isAddModalOpen = signal<boolean>(false);
  newItemName = signal<string>('');
  newItemIsClientSide = signal<boolean>(true);
  newItemTimeZoneNote = signal<string>('');

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
      }
    });
  }

  loadItems(id: number) {
    this.isLoading.set(true);
    this.readinessService.getAccessItems(id).subscribe({
      next: (items) => {
        this.accessItems.set(items);
        this.itemsUpdated.emit(items);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load access items', err);
        this.isLoading.set(false);
      }
    });
  }

  onStatusChange(item: AccessItemDto, event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const newStatusNum = parseInt(selectElement.value, 10) as AccessItemStatus;

    this.readinessService.updateAccessItemStatus(item.accessItemId, { status: newStatusNum }).subscribe({
      next: (updatedItem) => {
        const updatedList = this.accessItems().map(i =>
          i.accessItemId === updatedItem.accessItemId ? updatedItem : i
        );
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      },
      error: (err) => {
        console.error('Failed to update status', err);
        // revert select to previous status if call fails
        this.loadItems(this.engagementId());
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Granted':
      case '2':
        return 'badge-granted';
      case 'Blocked':
      case '3':
        return 'badge-blocked';
      default:
        return 'badge-inprogress';
    }
  }

  getStatusNumericValue(status: string): number {
    if (status === 'Granted' || status === '2') return AccessItemStatus.Granted;
    if (status === 'Blocked' || status === '3') return AccessItemStatus.Blocked;
    return AccessItemStatus.InProgress;
  }

  openAddModal() {
    this.newItemName.set('');
    this.newItemIsClientSide.set(true);
    this.newItemTimeZoneNote.set('');
    this.isAddModalOpen.set(true);
  }

  closeAddModal() {
    this.isAddModalOpen.set(false);
  }

  submitAddItem() {
    if (!this.newItemName().trim()) return;

    this.readinessService.addCustomAccessItem(this.engagementId(), {
      name: this.newItemName().trim(),
      isClientSide: this.newItemIsClientSide(),
      timeZoneNote: this.newItemTimeZoneNote().trim() || undefined
    }).subscribe({
      next: (newItem) => {
        const updatedList = [...this.accessItems(), newItem];
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.closeAddModal();
      },
      error: (err) => {
        console.error('Failed to add custom access item', err);
      }
    });
  }
}
