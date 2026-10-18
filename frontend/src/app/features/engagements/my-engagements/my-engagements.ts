import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EngagementService } from '../services/engagement.service';
import { EngagementSortBy, EngagementListItem } from '../models/engagement.model';
import { AppContextService } from '@shared/services/app-context';
import { EngagementsComponent } from '../pages/createengagment/engagements';
import { ENGAGEMENT_MESSAGES, USER_ROLES, APP_ROUTES, ENGAGEMENT_ROUTES } from '@shared/constants/app.constants';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { ReadinessCircleComponent } from '@shared/readiness-circle/readiness-circle';

interface MyEngagementRow extends EngagementListItem {
  displayReadinessScore: number;
  isOverdue: boolean;
}

type SortOption = 'kickoff' | 'readiness' | 'client';

const SORT_OPTIONS: SelectOption[] = [
  { value: 'kickoff', label: 'Kickoff date' },
  { value: 'readiness', label: 'Readiness score' }
];

@Component({
  selector: 'app-my-engagements',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    EngagementsComponent,
    AlertComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    PaginatorComponent,
    SelectComponent,
    PageHeaderComponent,
    LoadingSpinnerComponent,
    ReadinessCircleComponent
  ],
  templateUrl: './my-engagements.html',
  styleUrl: './my-engagements.scss'
})
export class MyEngagementsComponent implements OnInit {
  private engagementService = inject(EngagementService);
  private appContext = inject(AppContextService);
  private router = inject(Router);

  readonly isPreSales = computed(() => this.appContext.user()?.role === USER_ROLES.PRE_SALES);
  readonly sortOptions = SORT_OPTIONS;
  readonly pageSizeOptions = [5, 10, 15];

  sortBy = signal<SortOption>('kickoff');
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  activeTab = signal<'Active' | 'Past'>('Active');

  pageSize = signal(5);
  page = signal(1);
  totalCount = signal(0);
  activeEngagementsCount = signal(0);
  pastEngagementsCount = signal(0);
  pagedEngagements = signal<MyEngagementRow[]>([]);

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));

  readonly showingStart = computed(() => {
    if (this.totalCount() === 0) return 0;
    return (this.page() - 1) * this.pageSize() + 1;
  });

  readonly showingEnd = computed(() => {
    return Math.min(this.page() * this.pageSize(), this.totalCount());
  });

  setTab(tabId: 'Active' | 'Past'): void {
    this.activeTab.set(tabId);
    this.page.set(1);
    this.loadEngagements();
  }

  ngOnInit(): void {
    this.loadEngagements();
  }

  loadEngagements(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.engagementService
      .getMyEngagementsPage(this.activeTab(), this.sortBy() as EngagementSortBy, this.page(), this.pageSize())
      .subscribe({
        next: (result) => {
          this.pagedEngagements.set(
            result.items.map((engagement) => ({
              ...engagement,
              displayReadinessScore: Math.round(engagement.readinessScore ?? 0),
              isOverdue: engagement.daysToKickoff < 0
            }))
          );
          this.totalCount.set(result.totalCount);
          this.activeEngagementsCount.set(result.activeCount);
          this.pastEngagementsCount.set(result.pastCount);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.errorMessage.set(ENGAGEMENT_MESSAGES.LOAD_ENGAGEMENTS_ERROR);
        }
      });
  }

  onSortChange(sortBy: unknown): void {
    this.sortBy.set(sortBy as SortOption);
    this.page.set(1);
    this.loadEngagements();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }
    this.page.set(page);
    this.loadEngagements();
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
    this.loadEngagements();
  }

  openEngagement(engagementId: number): void {
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, engagementId, ENGAGEMENT_ROUTES.WORKSPACE, ENGAGEMENT_ROUTES.OVERVIEW]);
  }

  formatKickoffDate(kickoffDate: string): string {
    const date = new Date(kickoffDate);
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
