import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '@shared/services/auth';
import { TECHNICAL_READINESS_ROLES } from '@shared/constants/technical-readiness.constants';
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
  private authService = inject(AuthService);

  private parentParamMap = toSignal(this.route.parent!.paramMap);

  isPreSales = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.PRE_SALES));
  isTechLead = computed(() => this.authService.hasRole(TECHNICAL_READINESS_ROLES.TECH_LEAD));

  canViewDayOneBanner = computed(() => !this.isPreSales());
  canViewAccessTracker = computed(() => !this.isPreSales());
  canViewEnvironmentTooling = computed(() => !this.isPreSales());
  canViewArchitectureValidation = computed(() => true);
  canViewBrownfieldReview = computed(() => !this.isPreSales() && this.isBrownfield());

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

  accessGrantedCount = computed(() =>
    this.accessItems().filter(i => this.isGranted(i.status)).length
  );
  accessInProgressCount = computed(() =>
    this.accessItems().filter(i => this.isInProgress(i.status)).length
  );
  accessBlockedCount = computed(() =>
    this.accessItems().filter(i => this.isBlocked(i.status)).length
  );
  accessTotalCount = computed(() => this.accessItems().length);

  // Tooling Metrics
  private isToolingReadyStatus(status: string | number): boolean {
    return status === EnvironmentToolingStatus[EnvironmentToolingStatus.Granted] || +status === EnvironmentToolingStatus.Granted;
  }
  private isToolingNotReadyStatus(status: string | number): boolean {
    return status === EnvironmentToolingStatus[EnvironmentToolingStatus.InProgress] || +status === EnvironmentToolingStatus.InProgress;
  }
  private isToolingBlockedStatus(status: string | number): boolean {
    return status === EnvironmentToolingStatus[EnvironmentToolingStatus.Blocked] || +status === EnvironmentToolingStatus.Blocked;
  }

  toolingReadyCount = computed(() =>
    this.toolingItems().filter(i => this.isToolingReadyStatus(i.status)).length
  );
  toolingNotReadyCount = computed(() =>
    this.toolingItems().filter(i => this.isToolingNotReadyStatus(i.status)).length
  );
  toolingBlockedCount = computed(() =>
    this.toolingItems().filter(i => this.isToolingBlockedStatus(i.status)).length
  );
  toolingTotalCount = computed(() => this.toolingItems().length);

  // Blockers Check
  hasAccessBlockers = computed(() =>
    this.accessBlockedCount() > 0
  );
  hasToolingBlockers = computed(() =>
    this.toolingBlockedCount() > 0
  );
  hasBlockers = computed(() => this.hasAccessBlockers() || this.hasToolingBlockers());

  // Brownfield Readiness
  brownfieldHighPriorityCount = computed(() =>
    this.brownfieldReadiness()?.highSeverityPendingCount ?? 0
  );
  isBrownfieldReady = computed(() =>
    !this.isBrownfield() || (this.brownfieldReadiness()?.isTechnicallyReady ?? true)
  );

  // Architecture Sizing & Risk
  isSizingRiskRaised = computed(() => this.architectureValidation()?.isRiskRaised ?? false);
  sizingRiskSummary = computed(() => this.architectureValidation()?.riskSummary ?? null);
  sizingDeltaPercentage = computed(() => this.architectureValidation()?.sizingDeltaPercentage ?? 0);
  isArchitectureReady = computed(() => !this.isSizingRiskRaised());

  // Total readiness check
  totalRequired = computed(() => this.accessTotalCount() + this.toolingTotalCount());
  totalReady = computed(() => this.accessGrantedCount() + this.toolingReadyCount());

  isDayOneReady = computed(() =>
    !this.hasBlockers() &&
    this.totalRequired() > 0 &&
    this.totalReady() === this.totalRequired() &&
    this.isBrownfieldReady() &&
    this.isArchitectureReady()
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

  shareSummary(): void {
    const id = this.engagementId();
    if (!id) return;
    window.open(`/engagements/${id}/day-one-summary-print`, '_blank');
  }
}
