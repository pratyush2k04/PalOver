import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { TECHNICAL_READINESS_ROUTES } from 'src/app/constants/technical-readiness.constants';
import {  EngagementService } from '../services/engagement.service';
import { KickoffReadinessDto, KickoffReadinessService } from 'src/app/services/kickoff-readiness.service';
import { ENGAGEMENT_WORKSPACE_CONFIG } from 'src/app/constants/engagements.constants';
import { EngagementInfoDto } from '../models/engagement.model';

interface WorkspaceTab {
  label: string;
  route: string;
  enabled: boolean;
}

@Component({
  selector: 'app-engagement-workspace',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './engagement-workspace.component.html',
  styleUrl: './engagement-workspace.component.scss'
})
export class EngagementWorkspaceComponent {
  private route = inject(ActivatedRoute);
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

  readonly tabs = signal<WorkspaceTab[]>([...ENGAGEMENT_WORKSPACE_CONFIG.DEFAULT_TABS]);

  activeTabIndex = signal<number>(0);

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

  onTabKeydown(event: KeyboardEvent) {
    const enabledIndexes = this.tabs()
      .map((tab, index) => ({ tab, index }))
      .filter(({ tab }) => tab.enabled)
      .map(({ index }) => index);

    if (enabledIndexes.length === 0) {
      return;
    }

    const currentPos = enabledIndexes.indexOf(this.activeTabIndex());

    if (event.key === 'ArrowRight' || event.key === 'ArrowDown') {
      event.preventDefault();
      const nextPos = (currentPos + 1) % enabledIndexes.length;
      this.focusTab(enabledIndexes[nextPos]);
    } else if (event.key === 'ArrowLeft' || event.key === 'ArrowUp') {
      event.preventDefault();
      const prevPos = (currentPos - 1 + enabledIndexes.length) % enabledIndexes.length;
      this.focusTab(enabledIndexes[prevPos]);
    }
  }

  private focusTab(index: number) {
    this.activeTabIndex.set(index);
    const tabElements = document.querySelectorAll<HTMLElement>('.tab-item');
    tabElements[index]?.focus();
    tabElements[index]?.click();
  }
}
