import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin, of, catchError, switchMap, map } from 'rxjs';
import { EngagementService } from '../services/engagement.service';
import { EngagementSortBy, EngagementListItem } from '../models/engagement.model';
import { AppContextService } from '@shared/services/app-context';
import { KickoffReadinessService } from '@shared/services/kickoff-readiness.service';
import { EngagementsComponent } from '../pages/createengagment/engagements';
import { ENGAGEMENT_MESSAGES, USER_ROLES, APP_ROUTES, ENGAGEMENT_ROUTES } from '@shared/constants/app.constants';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { ButtonComponent } from '@common/components/button/button';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
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

const PAGE_SIZE = 4;

@Component({
  selector: 'app-my-engagements',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    EngagementsComponent,
    AlertComponent,
    EmptyStateComponent,
    ButtonComponent,
    StatusBadgeComponent,
    PaginatorComponent,
    SelectComponent,
    ReadinessCircleComponent
  ],
  templateUrl: './my-engagements.html',
  styleUrl: './my-engagements.scss'
})
export class MyEngagementsComponent implements OnInit {
  private engagementService = inject(EngagementService);
  private kickoffReadinessService = inject(KickoffReadinessService);
  private appContext = inject(AppContextService);
  private router = inject(Router);

  readonly isPreSales = computed(() => this.appContext.user()?.role === USER_ROLES.PRE_SALES);
  readonly sortOptions = SORT_OPTIONS;

  sortBy = signal<SortOption>('kickoff');
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  engagements = signal<MyEngagementRow[]>([]);
  currentPage = signal(1);

  activeCount = computed(() => this.engagements().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.activeCount() / PAGE_SIZE)));
  pagedEngagements = computed(() => {
    const start = (this.currentPage() - 1) * PAGE_SIZE;
    return this.engagements().slice(start, start + PAGE_SIZE);
  });

  ngOnInit(): void {
    this.loadEngagements();
  }

  loadEngagements(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.engagementService
      .getMyEngagements(this.sortBy() as EngagementSortBy)
      .pipe(
        switchMap((response) => {
          const list = response.engagements;
          if (!list || list.length === 0) {
            return of([]);
          }

          const readinessRequests = list.map((engagement) =>
            this.kickoffReadinessService.getReadiness(engagement.id).pipe(
              catchError(() => of({ score: 0 }))
            )
          );

          return forkJoin(readinessRequests).pipe(
            map((readinessResults) =>
              list.map((engagement, idx) => ({
                ...engagement,
                displayReadinessScore: Math.round(readinessResults[idx]?.score ?? 0),
                isOverdue: engagement.daysToKickoff < 0
              }))
            )
          );
        })
      )
      .subscribe({
        next: (rows) => {
          this.isLoading.set(false);

          if (this.sortBy() === 'readiness') {
            rows.sort((a, b) => b.displayReadinessScore - a.displayReadinessScore);
          }

          this.engagements.set(rows);
          this.currentPage.set(1);
        },
        error: () => {
          this.isLoading.set(false);
          this.errorMessage.set(ENGAGEMENT_MESSAGES.LOAD_ENGAGEMENTS_ERROR);
        }
      });
  }

  onSortChange(sortBy: unknown): void {
    this.sortBy.set(sortBy as SortOption);
    this.loadEngagements();
  }

  goToPage(page: number): void {
    this.currentPage.set(Math.min(Math.max(1, page), this.totalPages()));
  }

  openEngagement(engagementId: number): void {
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, engagementId, ENGAGEMENT_ROUTES.WORKSPACE]);
  }

  formatKickoffDate(kickoffDate: string): string {
    const date = new Date(kickoffDate);
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
