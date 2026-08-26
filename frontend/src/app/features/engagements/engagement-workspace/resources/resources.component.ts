import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '@shared/services/auth';
import { ButtonComponent } from '@common/components/button/button';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { AlertComponent } from '@common/components/alert/alert';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { DialogService } from '@common/services/dialog/dialog.service';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { ResourceAllocationItem, ResourceRequestItem } from '../../models/resource-request.model';
import { EngagementInfoDto } from '../../models/engagement.model';
import { ResourceRequestService } from '../../services/resource-request.service';
import { EngagementService } from '../../services/engagement.service';
import {
  RaiseResourceRequestDialogComponent,
  RaiseResourceRequestDialogData,
} from './raise-resource-request-dialog/raise-resource-request-dialog.component';
import {
  AllocateResourcesDialogComponent,
  AllocateResourcesDialogData,
} from './allocate-resources-dialog/allocate-resources-dialog.component';
import {
  ConfirmAllocationDialogComponent,
  ConfirmAllocationDialogData,
} from './confirm-allocation-dialog/confirm-allocation-dialog.component';
import {
  RemoveAllocationDialogComponent,
  RemoveAllocationDialogData,
} from './remove-allocation-dialog/remove-allocation-dialog.component';
import { SectionHeaderComponent } from '@common/components/section-header/section-header';

@Component({
  selector: 'app-resources',
  standalone: true,
  imports: [
    CommonModule,
    ButtonComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    AlertComponent,
    SectionHeaderComponent,
  ],
  templateUrl: './resources.component.html',
  styleUrl: './resources.component.scss',
})
export class ResourcesComponent {
  private route = inject(ActivatedRoute);
  private resourceRequestService = inject(ResourceRequestService);
  private engagementService = inject(EngagementService);
  private authService = inject(AuthService);
  private dialog = inject(DialogService);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  readonly isLeadershipOrAdmin = computed(
    () => this.authService.hasRole('Leadership') || this.authService.isAdmin(),
  );
  // Raising a request has no Admin bypass on the backend (CreateResourceRequestAsync requires
  // the current user to actually be the PM assigned to the engagement) — gate the button to match.
  readonly isProjectManager = computed(() => this.authService.hasRole('Project Management'));

  private paramMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  readonly engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  requests = signal<ResourceRequestItem[]>([]);
  engagementInfo = signal<EngagementInfoDto | null>(null);
  readonly isLoading = signal<boolean>(true);

  readonly unconfirmedCount = computed(
    () => this.requests().filter((r) => r.status !== 'Confirmed').length,
  );
  readonly hasUnconfirmedRequests = computed(() => this.unconfirmedCount() > 0);
  readonly billingStartDate = computed(() => this.engagementInfo()?.billingStartDate ?? null);

  readonly daysToBilling = computed<number | null>(() => {
    const billingDateStr = this.billingStartDate();
    if (!billingDateStr) return null;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const [year, month, day] = billingDateStr.split('-').map(Number);
    const billingDate = new Date(Date.UTC(year, month - 1, day));
    const utcToday = new Date(Date.UTC(today.getFullYear(), today.getMonth(), today.getDate()));
    const diffMs = billingDate.getTime() - utcToday.getTime();
    return Math.round(diffMs / (1000 * 60 * 60 * 24));
  });

  readonly showWarningBanner = computed(() => {
    const days = this.daysToBilling();
    return days !== null && days <= 3 && this.hasUnconfirmedRequests();
  });

  readonly formattedBillingDate = computed(() => {
    const date = this.billingStartDate();
    return date ? this.formatDisplayDate(date) : '';
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadData(id);
      }
    });
  }

  loadData(engagementId: number): void {
    this.engagementService.getEngagementInfo(engagementId).subscribe({
      next: (info) => this.engagementInfo.set(info),
      error: () => this.engagementInfo.set(null),
    });

    this.isLoading.set(true);
    this.resourceRequestService.getResourceRequests(engagementId).subscribe({
      next: (data) => {
        this.requests.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.requests.set([]);
        this.isLoading.set(false);
      },
    });
  }

  openRaiseModal(): void {
    const id = this.engagementId();
    if (!id) return;

    const dialogRef = this.dialog.open<
      RaiseResourceRequestDialogComponent,
      RaiseResourceRequestDialogData,
      ResourceRequestItem | null
    >(RaiseResourceRequestDialogComponent, {
      data: {
        engagementId: id,
        billingStartDate: this.billingStartDate(),
      },
    });

    dialogRef.closed$.subscribe((created) => {
      if (created) {
        this.loadData(id);
      }
    });
  }

  openAllocateModal(request: ResourceRequestItem): void {
    const id = this.engagementId();
    if (!id) return;

    const dialogRef = this.dialog.open<
      AllocateResourcesDialogComponent,
      AllocateResourcesDialogData,
      ResourceRequestItem | null
    >(AllocateResourcesDialogComponent, {
      data: {
        engagementId: id,
        requestId: request.id,
        peopleNeeded: request.peopleNeeded,
        standardRoleLabel: request.standardRole,
        existingAllocations: request.allocations,
      },
    });

    dialogRef.closed$.subscribe((updated) => {
      if (updated) {
        this.loadData(id);
      }
    });
  }

  openConfirmDialog(request: ResourceRequestItem, allocation: ResourceAllocationItem): void {
    const id = this.engagementId();
    if (!id) return;

    const dialogRef = this.dialog.open<
      ConfirmAllocationDialogComponent,
      ConfirmAllocationDialogData,
      ResourceRequestItem | null
    >(ConfirmAllocationDialogComponent, {
      data: {
        engagementId: id,
        requestId: request.id,
        allocationId: allocation.id,
        allocationName: allocation.name,
      },
    });

    dialogRef.closed$.subscribe((updated) => {
      if (updated) {
        this.loadData(id);
      }
    });
  }

  openRemoveDialog(request: ResourceRequestItem, allocation: ResourceAllocationItem): void {
    const id = this.engagementId();
    if (!id) return;

    const dialogRef = this.dialog.open<
      RemoveAllocationDialogComponent,
      RemoveAllocationDialogData,
      ResourceRequestItem | null
    >(RemoveAllocationDialogComponent, {
      data: {
        engagementId: id,
        requestId: request.id,
        allocationId: allocation.id,
        allocationName: allocation.name,
      },
    });

    dialogRef.closed$.subscribe((updated) => {
      if (updated) {
        this.loadData(id);
      }
    });
  }

  statusTone(status: string): StatusBadgeTone {
    switch (status) {
      case 'Allocated':
        return 'info';
      case 'Confirmed':
        return 'success';
      default:
        return 'neutral';
    }
  }

  formatDisplayDate(dateStr: string): string {
    if (!dateStr) return '';
    try {
      const [year, month, day] = dateStr.split('-').map(Number);
      if (!year || !month || !day) return dateStr;
      const date = new Date(Date.UTC(year, month - 1, day));
      const months = [
        'Jan',
        'Feb',
        'Mar',
        'Apr',
        'May',
        'Jun',
        'Jul',
        'Aug',
        'Sep',
        'Oct',
        'Nov',
        'Dec',
      ];
      return `${day} ${months[date.getUTCMonth()]} ${year}`;
    } catch {
      return dateStr;
    }
  }
}
