import { Component, inject, input, output, signal, computed, effect, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { catchError, of } from 'rxjs';
import { TECHNICAL_READINESS_ROLES } from '../../../../constants/app.constants';
import { TechnicalReadinessService } from '../../services/technical-readiness.service';
import { AuthService } from 'src/app/services/auth';
import { AccessItemDto, AccessItemStatus } from '../../models/technical-readiness.model';
import { KickoffReadinessDto, KickoffReadinessService } from '../../../../services/kickoff-readiness.service';
import { FlaggedItemsBannerComponent, FlaggedItem } from '@shared/flagged-items-banner/flagged-items-banner.component';

import { InlineSVGModule } from 'ng-inline-svg-2';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { CheckboxComponent } from '@common/components/form/checkbox/checkbox';

@Component({
  selector: 'app-access-tracker',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    FlaggedItemsBannerComponent,
    StatusBadgeComponent,
    ButtonComponent,
    InputComponent,
    TextareaComponent,
    CheckboxComponent
  ],
  templateUrl: './access-tracker.component.html',
  styleUrl: './access-tracker.component.scss'
})
export class AccessTrackerComponent {
  engagementId = input.required<number>();
  itemsUpdated = output<AccessItemDto[]>();

  private readinessService = inject(TechnicalReadinessService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);

  accessItems = signal<AccessItemDto[]>([]);
  isLoading = signal<boolean>(true);
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));

  kickoffReadiness = signal<KickoffReadinessDto | null>(null);
  blockedItems = computed(() => this.accessItems().filter(i => i.status === 'Blocked'));
  accessTrackerFlaggedItems = computed<FlaggedItem[]>(() => [
    { label: 'access item(s) blocked', count: this.blockedItems().length }
  ]);

  // Add Item Modal state
  isAddModalOpen = signal<boolean>(false);
  newItemName = signal<string>('');
  newItemIsClientSide = signal<boolean>(true);
  newItemTimeZoneNote = signal<string>('');
  nameError = signal<string>('');

  // Block Modal state
  isBlockModalOpen = signal<boolean>(false);
  blockReasonText = signal<string>('');
  blockReasonError = signal<string>('');
  itemToBlock = signal<AccessItemDto | null>(null);

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
        this.loadKickoffReadiness(id);
      }
    });
  }

  private loadKickoffReadiness(id: number) {
    this.kickoffReadinessService.getReadiness(id).pipe(
      takeUntilDestroyed(this.destroyRef),
      catchError(() => of(null))
    ).subscribe(data => this.kickoffReadiness.set(data));
  }

  loadItems(id: number) {
    this.isLoading.set(true);
    this.readinessService.getAccessItems(id).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (items) => {
        const finalItems = items && items.length > 0 ? items : this.getDummyItems();
        this.accessItems.set(finalItems);
        this.itemsUpdated.emit(finalItems);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.warn('Failed to load access items from API, falling back to seed items', err);
        const dummyItems = this.getDummyItems();
        this.accessItems.set(dummyItems);
        this.itemsUpdated.emit(dummyItems);
        this.isLoading.set(false);
      }
    });
  }

  private getDummyItems(): AccessItemDto[] {
    return [
      {
        accessItemId: 101,
        name: 'Client VPN access',
        isClientSide: true,
        ownerId: 1,
        ownerFullName: 'Ananth Vishwanatha',
        status: 'InProgress',
        timeZoneNote: 'Client is IST, no overlap issue',
        expectedTurnaroundDays: 5,
        daysUntilBillingDate: 6,
        isOverdueWithClient: false
      },
      {
        accessItemId: 102,
        name: 'Client Git repository',
        isClientSide: true,
        ownerId: 2,
        ownerFullName: 'Rahul Verma',
        status: 'Granted',
        timeZoneNote: 'Granted on Day-2',
        expectedTurnaroundDays: 3,
        daysUntilBillingDate: 6,
        isOverdueWithClient: false
      },
      {
        accessItemId: 103,
        name: 'Client Jira / tracker access',
        isClientSide: true,
        ownerId: 3,
        ownerFullName: 'Asha Rao',
        status: 'Blocked',
        timeZoneNote: 'Awaiting client admin approval',
        expectedTurnaroundDays: 7,
        daysUntilBillingDate: 2,
        isOverdueWithClient: true,
        blockReason: 'Client IT portal down'
      }
    ];
  }

  onStatusChange(item: AccessItemDto, event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const newStatusNum = parseInt(selectElement.value, 10) as AccessItemStatus;

    if (newStatusNum === AccessItemStatus.Blocked) {
      // Temporarily revert select dropdown display until block reason confirmed
      selectElement.value = this.getStatusNumericValue(item.status).toString();
      this.itemToBlock.set(item);
      this.blockReasonText.set(item.blockReason || '');
      this.blockReasonError.set('');
      this.isBlockModalOpen.set(true);
      return;
    }

    this.updateStatus(item, newStatusNum);
  }

  closeBlockModal() {
    this.isBlockModalOpen.set(false);
    this.itemToBlock.set(null);
    this.blockReasonText.set('');
    this.blockReasonError.set('');
  }

  submitBlockReason() {
    const item = this.itemToBlock();
    if (!item) return;

    const reason = this.blockReasonText().trim();
    if (!reason) {
      this.blockReasonError.set('Reason is required to block an item');
      return;
    }

    this.blockReasonError.set('');
    this.updateStatus(item, AccessItemStatus.Blocked, reason);
    this.closeBlockModal();
  }

  private updateStatus(item: AccessItemDto, newStatusNum: AccessItemStatus, blockReason?: string) {
    const statusTextMap: Record<number, string> = {
      [AccessItemStatus.InProgress]: 'InProgress',
      [AccessItemStatus.Granted]: 'Granted',
      [AccessItemStatus.Blocked]: 'Blocked'
    };

    const payload = {
      status: newStatusNum,
      blockReason: newStatusNum === AccessItemStatus.Blocked ? blockReason : undefined
    };

    this.readinessService.updateAccessItemStatus(item.accessItemId, payload).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (updatedItem) => {
        const updatedList = this.accessItems().map(i =>
          i.accessItemId === updatedItem.accessItemId ? updatedItem : i
        );
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
      },
      error: (err) => {
        console.warn('API update status failed, updating local state for testing', err);
        const updatedList = this.accessItems().map(i => {
          if (i.accessItemId === item.accessItemId) {
            return {
              ...i,
              status: statusTextMap[newStatusNum] || 'InProgress',
              blockReason: newStatusNum === AccessItemStatus.Blocked ? blockReason : undefined
            };
          }
          return i;
        });
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
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

  getStatusTone(status: string): StatusBadgeTone {
    switch (status) {
      case 'Granted':
      case '2':
        return 'success';
      case 'Blocked':
      case '3':
        return 'danger';
      default:
        return 'info';
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
      name: this.newItemName().trim(),
      isClientSide: this.newItemIsClientSide(),
      timeZoneNote: this.newItemTimeZoneNote().trim() || undefined,
      ownerId: 1 // Default to Rahul Verma (ID 1)
    };

    this.readinessService.addCustomAccessItem(this.engagementId(), requestData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (newItem) => {
        const updatedList = [...this.accessItems(), newItem];
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.closeAddModal();
      },
      error: (err) => {
        console.warn('API add item failed, adding item locally for testing', err);
        const localNewItem: AccessItemDto = {
          accessItemId: Date.now(),
          name: requestData.name,
          isClientSide: requestData.isClientSide,
          ownerId: 1,
          ownerFullName: 'Rahul Verma',
          status: 'InProgress',
          timeZoneNote: requestData.timeZoneNote,
          daysUntilBillingDate: 5,
          isOverdueWithClient: false
        };
        const updatedList = [...this.accessItems(), localNewItem];
        this.accessItems.set(updatedList);
        this.itemsUpdated.emit(updatedList);
        this.closeAddModal();
      }
    });
  }
}
