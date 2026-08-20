import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { EngagementService } from '../services/engagement.service';
import { KickoffReadinessDto, KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { ENGAGEMENT_WORKSPACE_CONFIG } from '@shared/constants/engagements.constants';
import { EngagementInfoDto, EngagementOwners } from '../models/engagement.model';
import { TabItem, TabsComponent } from '@common/components/tabs/tabs';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterOutlet, TabsComponent, InlineSVGModule],
  templateUrl: './engagement-workspace.component.html',
  styleUrl: './engagement-workspace.component.scss'
})
export class EngagementWorkspaceComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private engagementService = inject(EngagementService);
  private kickoffReadinessService = inject(KickoffReadinessService);

  private paramMap = toSignal(this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagement = signal<EngagementInfoDto | null>(null);
  readiness = signal<KickoffReadinessDto | null>(null);
  owners = signal<EngagementOwners | null>(null);
  isLoading = signal<boolean>(true);

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

  readonly ringColor = computed(() => {
    const score = this.scorePercent();
    if (score < 10) return '#dc2626'; // red ($color-danger)
    if (score > 90) return '#16a34a'; // green ($color-success)
    return '#f59e0b'; // orange ($color-accent-amber)
  });

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

    this.engagementService.getEngagementOwners(id).subscribe({
      next: (data) => this.owners.set(data),
      error: () => this.owners.set(null)
    });
  }
}

