import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { EngagementService } from '../services/engagement.service';
import { TechnicalReadinessService } from '../services/technical-readiness.service';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { ENGAGEMENT_WORKSPACE_CONFIG } from '@shared/constants/engagements.constants';
import { USER_ROLES } from '@shared/constants/users.constants';
import { EngagementInfoDto } from '../models/engagement.model';
import { ENGAGEMENT_ROUTES } from '@shared/constants/routes.constants';
import { TabItem, TabsComponent } from '@common/components/tabs/tabs';
import { ReadinessCircleComponent } from '@shared/readiness-circle/readiness-circle';
import { DayOneSignOffDialogComponent } from './components/day-one-sign-off-dialog/day-one-sign-off-dialog.component';
import { ButtonComponent } from '@common/components/button/button';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogService } from '@common/services/dialog/dialog.service';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe, 
    RouterOutlet,
    TabsComponent,
    StatusBadgeComponent,
    ButtonComponent,
    InlineSVGModule,
    ReadinessCircleComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './engagement-workspace.component.html',
  styleUrl: './engagement-workspace.component.scss'
})
export class EngagementWorkspaceComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private engagementService = inject(EngagementService);
  private technicalReadinessService = inject(TechnicalReadinessService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private dialogService = inject(DialogService);

  private paramMap = toSignal(this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagement = signal<EngagementInfoDto | null>(null);
  readiness = this.kickoffReadinessService.readiness;
  readonly owners = this.engagementService.owners;
  isLoading = signal<boolean>(true);

  readonly isAuthorizedForSignOff = computed(() => {
    const user = this.authService.currentUser();
    if (!user) return false;
    return (
      this.authService.hasRole(USER_ROLES.LEADERSHIP) ||
      this.authService.hasRole(USER_ROLES.PROJECT_MANAGEMENT)
    );
  });

  readonly isLeadership = computed(() => {
    const user = this.authService.currentUser();
    if (!user) return false;
    return this.authService.hasRole(USER_ROLES.LEADERSHIP);
  });

  readonly tabs = computed<TabItem[]>(() => {
    const isSignedOff = !!this.engagement()?.dayOneSignOff;

    return ENGAGEMENT_WORKSPACE_CONFIG.DEFAULT_TABS
      .filter((tab) => {
        if (tab.route === ENGAGEMENT_ROUTES.SIGN_OFF) {
          // Hidden before sign-off, but once signed off, visible to all roles (including Pre-Sales & Tech Lead)
          return isSignedOff;
        }
        return true;
      })
      .map((tab) => {
        let enabled = tab.enabled;
        if (tab.route === ENGAGEMENT_ROUTES.SIGN_OFF) {
          enabled = isSignedOff;
        }
        return { id: tab.route, ...tab, enabled };
      });
  });

  readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map(() => this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly activeTabId = computed(
    () => this.tabs().find((tab) => this.currentUrl().includes(`/${tab.route}`))?.id ?? null
  );

  readonly scorePercent = computed(() => {
    const r = this.readiness();
    return r ? Math.round(r.score) : 0;
  });

  readonly isBrownfield = computed(() =>
    (this.engagement()?.engagementType ?? '').toLowerCase().includes('brown')
  );

  readonly isGreenfield = computed(() =>
    (this.engagement()?.engagementType ?? '').toLowerCase().includes('green')
  );

  readonly isFullyReady = computed(() => {
    return this.scorePercent() === 100;
  });



  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadEngagement(id);
      }
    });
  }

  private loadEngagement(id: number, onDone?: () => void) {
    this.isLoading.set(true);
    this.engagementService.getEngagementInfo(id).subscribe({
      next: (data) => {
        this.engagement.set(data);
        this.isLoading.set(false);
        onDone?.();
      },
      error: (error) => {
        console.error(error);
        this.engagement.set(null);
        this.isLoading.set(false);
        onDone?.();
      }
    });

    this.kickoffReadinessService.track(id);

    this.engagementService.getEngagementOwners(id).subscribe({
      error: () => {}
    });
  }

  openSignOffDialog(): void {
    const id = this.engagementId();
    if (!id) return;

    this.technicalReadinessService.getTechnicalSummary(id).subscribe({
      next: (summary) => {
        this.launchSignOffDialog(id, summary.isAbleToStartDayOne);
      },
      error: () => {
        this.launchSignOffDialog(id, false);
      }
    });
  }

  private launchSignOffDialog(engagementId: number, isAbleToStartDayOne: boolean): void {
    const dialogRef = this.dialogService.open(DayOneSignOffDialogComponent, {
      data: {
        engagementId,
        isAbleToStartDayOne,
        isLeadership: this.isLeadership(),
        initialBillingStartDate: this.engagement()?.billingStartDate || ''
      }
    });

    dialogRef.closed$.subscribe((result) => {
      if (!result) return;

      this.loadEngagement(engagementId, () => this.toastService.success('Day-1 successfully signed off!'));
    });
  }

  openReadinessOnePager(): void {
    const engagementId = this.engagementId();
    if (!engagementId) return;

    window.open(`/engagements/${engagementId}/${ENGAGEMENT_ROUTES.READINESS_ONE_PAGER_PRINT}`, '_blank');
  }
}

