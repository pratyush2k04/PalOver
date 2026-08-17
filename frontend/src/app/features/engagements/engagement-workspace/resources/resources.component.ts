import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { ButtonComponent } from '@common/components/button/button';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { DialogService } from '@common/services/dialog/dialog.service';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { ResourceRequestItem } from '../../models/resource-request.model';
import { EngagementInfoDto } from '../../models/engagement.model';
import { ResourceRequestService } from '../../services/resource-request.service';
import { EngagementService } from '../../services/engagement.service';
import {
  RaiseResourceRequestDialogComponent,
  RaiseResourceRequestDialogData,
} from './raise-resource-request-dialog/raise-resource-request-dialog.component';

@Component({
  selector: 'app-resources',
  standalone: true,
  imports: [CommonModule, ButtonComponent, EmptyStateComponent],
  templateUrl: './resources.component.html',
  styleUrl: './resources.component.scss',
})
export class ResourcesComponent {
  private route = inject(ActivatedRoute);
  private resourceRequestService = inject(ResourceRequestService);
  private engagementService = inject(EngagementService);
  private dialog = inject(DialogService);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  private paramMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  readonly engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  requests = signal<ResourceRequestItem[]>([]);
  engagementInfo = signal<EngagementInfoDto | null>(null);

  readonly unconfirmedCount = computed(
    () => this.requests().filter((r) => r.status !== 'Confirmed').length,
  );
  readonly hasUnconfirmedRequests = computed(() => this.unconfirmedCount() > 0);
  readonly billingStartDate = computed(() => this.engagementInfo()?.billingStartDate ?? null);
  readonly showWarningBanner = computed(
    () => this.hasUnconfirmedRequests() && !!this.billingStartDate(),
  );

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

    this.resourceRequestService.getResourceRequests(engagementId).subscribe({
      next: (data) => this.requests.set(data),
      error: () => this.requests.set([]),
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
