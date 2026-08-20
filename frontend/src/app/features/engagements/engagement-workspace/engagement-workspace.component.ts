import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { EngagementService } from '../services/engagement.service';
import { TechnicalReadinessService } from '../services/technical-readiness.service';
import { KickoffReadinessDto, KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { AuthService } from '@shared/services/auth';
import { ToastService } from '@common/services/toast/toast';
import { ENGAGEMENT_WORKSPACE_CONFIG } from '@shared/constants/engagements.constants';
import { DayOneSignOffRequest, EngagementInfoDto } from '../models/engagement.model';
import { TabItem, TabsComponent } from '@common/components/tabs/tabs';
import { ButtonComponent } from '@common/components/button/button';
import { DayOneSignOffDialogComponent } from './components/day-one-sign-off-dialog/day-one-sign-off-dialog.component';

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    TabsComponent,
    ButtonComponent,
    DayOneSignOffDialogComponent
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

  private paramMap = toSignal(this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagement = signal<EngagementInfoDto | null>(null);
  readiness = signal<KickoffReadinessDto | null>(null);
  isLoading = signal<boolean>(true);

  // Day-1 Sign-off state
  readonly isSignOffDialogOpen = signal<boolean>(false);
  readonly isSigningOff = signal<boolean>(false);
  readonly isAbleToStartDayOne = signal<boolean>(true);

  readonly isAuthorizedForSignOff = computed(() => {
    const user = this.authService.currentUser();
    if (!user) return false;
    return user.role === 'Leadership' || user.role === 'ProjectManagement';
  });

  readonly isLeadership = computed(() => {
    return this.authService.currentUser()?.role === 'Leadership';
  });

  readonly tabs = signal<TabItem[]>(
    ENGAGEMENT_WORKSPACE_CONFIG.DEFAULT_TABS.map((tab) => ({ id: tab.route, ...tab }))
  );

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

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadEngagement(id);
      }
    });
  }

  private loadEngagement(id: number) {
    this.isLoading.set(true);
    this.engagementService.getEngagementInfo(id).subscribe({
      next: (data) => {
        this.engagement.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.engagement.set(null);
        this.isLoading.set(false);
      }
    });

    this.kickoffReadinessService.getReadiness(id).subscribe({
      next: (data) => this.readiness.set(data),
      error: () => this.readiness.set(null)
    });
  }

  openSignOffDialog(): void {
    const id = this.engagementId();
    if (!id) return;

    this.technicalReadinessService.getTechnicalSummary(id).subscribe({
      next: (summary) => {
        this.isAbleToStartDayOne.set(summary.isAbleToStartDayOne);
        this.isSignOffDialogOpen.set(true);
      },
      error: () => {
        this.isAbleToStartDayOne.set(false);
        this.isSignOffDialogOpen.set(true);
      }
    });
  }

  onSignOffConfirmed(payload: DayOneSignOffRequest): void {
    const id = this.engagementId();
    if (!id) return;

    this.isSigningOff.set(true);
    this.engagementService.signOffDayOne(id, payload).subscribe({
      next: () => {
        this.isSigningOff.set(false);
        this.isSignOffDialogOpen.set(false);
        this.toastService.success('Day-1 successfully signed off!');
        this.loadEngagement(id);
      },
      error: () => {
        this.isSigningOff.set(false);
      }
    });
  }

  onSignOffCancelled(): void {
    this.isSignOffDialogOpen.set(false);
  }
}
