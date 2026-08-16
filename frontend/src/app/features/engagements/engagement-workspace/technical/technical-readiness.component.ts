import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { TECHNICAL_READINESS_ALERTS } from '@shared/constants/app.constants';
import { AccessItemDto } from '@features/engagements/models/access-tracker/access-tracker.dtos';
import { AccessItemStatus } from '@features/engagements/models/access-tracker/access-tracker.enums';
import { EnvironmentToolingItemDto } from '@features/engagements/models/environment-tooling/environment-tooling.dtos';
import { EnvironmentToolingStatus } from '@features/engagements/models/environment-tooling/environment-tooling.enums';
import { BrownfieldReadinessResponse } from '@features/engagements/models/brownfield-review/brownfield-review.dtos';
import { ArchitectureValidationDto } from '@features/engagements/models/architecture-validation/architecture-validation.dtos';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { EngagementInfoDto } from '@features/engagements/models/engagement.model';
import { AccessTrackerComponent } from './access-tracker/access-tracker.component';
import { EnvironmentToolingComponent } from './environment-tooling/environment-tooling.component';
import { BrownfieldReviewComponent } from './brownfield-review/brownfield-review.component';
import { ArchitectureValidationComponent } from './architecture-validation/architecture-validation.component';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-technical-readiness',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    AccessTrackerComponent,
    EnvironmentToolingComponent,
    BrownfieldReviewComponent,
    ArchitectureValidationComponent
  ],
  templateUrl: './technical-readiness.component.html',
  styleUrl: './technical-readiness.component.scss'
})
export class TechnicalReadinessComponent {
  private route = inject(ActivatedRoute);
  private engagementService = inject(EngagementService);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  engagementId = computed(() => {
    const idStr = this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagementInfo = signal<EngagementInfoDto | null>(null);
  accessItems = signal<AccessItemDto[]>([]);
  toolingItems = signal<EnvironmentToolingItemDto[]>([]);
  brownfieldReadiness = signal<BrownfieldReadinessResponse | null>(null);
  architectureValidation = signal<ArchitectureValidationDto | null>(null);

  isBrownfield = computed(() => {
    const type = this.engagementInfo()?.engagementType ?? '';
    return type.toLowerCase().includes('brownfield');
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.engagementService.getEngagementInfo(id).subscribe({
          next: (info) => this.engagementInfo.set(info),
          error: () => this.engagementInfo.set(null)
        });
      }
    });
  }

  // Access Metrics
  private isGranted(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.Granted] || +status === AccessItemStatus.Granted;
  }
  private isInProgress(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.InProgress] || +status === AccessItemStatus.InProgress;
  }
  private isBlocked(status: string | number): boolean {
    return status === AccessItemStatus[AccessItemStatus.Blocked] || +status === AccessItemStatus.Blocked;
  }

  accessApplicableItems = computed(() =>
    this.accessItems().filter(i => this.isGranted(i.status) || this.isInProgress(i.status))
  );
  accessGrantedCount = computed(() =>
    this.accessApplicableItems().filter(i => this.isGranted(i.status)).length
  );
  accessTotalCount = computed(() => this.accessApplicableItems().length);

  // Tooling Metrics
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

  // Brownfield Readiness
  isBrownfieldReady = computed(() =>
    !this.isBrownfield() || (this.brownfieldReadiness()?.isTechnicallyReady ?? true)
  );

  // Architecture Sizing & Risk
  isSizingRiskRaised = computed(() => this.architectureValidation()?.isRiskRaised ?? false);
  sizingRiskSummary = computed(() => this.architectureValidation()?.riskSummary ?? null);
  isArchitectureReady = computed(() => !this.isSizingRiskRaised());

  // Total readiness check
  totalRequired = computed(() => this.accessTotalCount() + this.toolingTotalCount());
  totalReady = computed(() => this.accessGrantedCount() + this.toolingReadyCount());

  isDayOneReady = computed(() =>
    !this.hasBlockers() &&
    this.totalRequired() > 0 &&
    this.totalReady() === this.totalRequired() &&
    this.isBrownfieldReady()
  );

  onAccessItemsUpdated(items: AccessItemDto[]) {
    this.accessItems.set(items);
  }

  onToolingItemsUpdated(items: EnvironmentToolingItemDto[]) {
    this.toolingItems.set(items);
  }

  onBrownfieldReadinessUpdated(res: BrownfieldReadinessResponse) {
    this.brownfieldReadiness.set(res);
  }

  onArchitectureValidationUpdated(val: ArchitectureValidationDto) {
    this.architectureValidation.set(val);
  }

  shareSummary() {
    alert(TECHNICAL_READINESS_ALERTS.SHARE_SUMMARY);
  }
}
