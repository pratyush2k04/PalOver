import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { EngagementService } from '../services/engagement.service';
import { KickoffReadinessDto, KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { ENGAGEMENT_WORKSPACE_CONFIG } from '@shared/constants/engagements.constants';
import { EngagementInfoDto } from '../models/engagement.model';
import { TabItem, TabsComponent } from '@common/components/tabs/tabs';

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [CommonModule, RouterOutlet, TabsComponent],
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
        console.log("Engagement loaded properly");
        console.log(data);
        this.engagement.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.log("Error loading engagement");
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
}
