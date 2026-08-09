import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { TECHNICAL_READINESS_ALERTS } from '../../../constants/app.constants';
import { AccessItemDto, AccessItemStatus } from '../models/technical-readiness.model';
import { EnvironmentToolingItemDto, EnvironmentToolingStatus } from '../models/environment-tooling.model';
import { AccessTrackerComponent } from './access-tracker/access-tracker.component';
import { EnvironmentToolingComponent } from './environment-tooling/environment-tooling.component';

import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-technical-readiness',
  standalone: true,
  imports: [CommonModule, InlineSVGModule, AccessTrackerComponent, EnvironmentToolingComponent],
  templateUrl: './technical-readiness.component.html',
  styleUrl: './technical-readiness.component.scss'
})
export class TechnicalReadinessComponent {
  private route = inject(ActivatedRoute);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  accessItems = signal<AccessItemDto[]>([]);
  toolingItems = signal<EnvironmentToolingItemDto[]>([]);

  // Helper check methods for AccessItemStatus using enum
  private isGranted(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.Granted] || +status === AccessItemStatus.Granted;
  }

  private isInProgress(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.InProgress] || +status === AccessItemStatus.InProgress;
  }

  private isBlocked(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.Blocked] || +status === AccessItemStatus.Blocked;
  }

  // Access Metrics (Calculated against InProgress and Granted only)
  accessApplicableItems = computed(() =>
    this.accessItems().filter(i => this.isGranted(i.status) || this.isInProgress(i.status))
  );
  accessGrantedCount = computed(() =>
    this.accessApplicableItems().filter(i => this.isGranted(i.status)).length
  );
  accessTotalCount = computed(() => this.accessApplicableItems().length);

  // Tooling Metrics (Not Applicable items are excluded from required total)
  toolingApplicableItems = computed(() =>
    this.toolingItems().filter(i =>
      +i.status !== EnvironmentToolingStatus.NotApplicable &&
      i.status !== (EnvironmentToolingStatus[EnvironmentToolingStatus.NotApplicable] as any)
    )
  );

  toolingReadyCount = computed(() =>
    this.toolingApplicableItems().filter(i =>
      +i.status === EnvironmentToolingStatus.Ready ||
      i.status === (EnvironmentToolingStatus[EnvironmentToolingStatus.Ready] as any)
    ).length
  );

  toolingTotalCount = computed(() => this.toolingApplicableItems().length);

  // Blockers Check
  hasAccessBlockers = computed(() =>
    this.accessItems().some(i => this.isBlocked(i.status))
  );

  hasBlockers = computed(() => this.hasAccessBlockers());

  // Total readiness check
  totalRequired = computed(() => this.accessTotalCount() + this.toolingTotalCount());
  totalReady = computed(() => this.accessGrantedCount() + this.toolingReadyCount());

  isDayOneReady = computed(() =>
    !this.hasBlockers() &&
    this.totalRequired() > 0 &&
    this.totalReady() === this.totalRequired()
  );

  onAccessItemsUpdated(items: AccessItemDto[]) {
    this.accessItems.set(items);
  }

  onToolingItemsUpdated(items: EnvironmentToolingItemDto[]) {
    this.toolingItems.set(items);
  }

  shareSummary() {
    alert(TECHNICAL_READINESS_ALERTS.SHARE_SUMMARY);
  }
}

