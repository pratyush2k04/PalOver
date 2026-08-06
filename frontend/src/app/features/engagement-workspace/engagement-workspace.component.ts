import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { EngagementInfoDto, EngagementService } from '../../services/engagement.service';
import { TabItem, TabsComponent } from '../../common/components/tabs/tabs';

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

  private paramMap = toSignal(this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id');
    return idStr ? +idStr : null;
  });

  engagement = signal<EngagementInfoDto | null>(null);
  isLoading = signal<boolean>(true);

  readonly tabs: TabItem[] = [
    { id: 'handover', label: 'Handover', route: 'handover' },
    { id: 'documents', label: 'Documents', route: 'documents' },
    { id: 'people', label: 'People', route: 'people', enabled: false },
    { id: 'checklist', label: 'Checklist', route: 'checklist' },
    { id: 'kickoff', label: 'Kickoff', route: 'kickoff', enabled: false },
    { id: 'resourcing', label: 'Resourcing', route: 'resourcing', enabled: false },
    { id: 'technical-readiness', label: 'Technical Readiness', route: 'technical-readiness', enabled: false },
    { id: 'risks', label: 'Risks', route: 'risks', enabled: false },
    { id: 'reporting', label: 'Reporting', route: 'reporting', enabled: false }
  ];

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(() => this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly activeTabId = computed(
    () => this.tabs.find((tab) => this.currentUrl().includes(`/${tab.route}`))?.id ?? null
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
  }
}
