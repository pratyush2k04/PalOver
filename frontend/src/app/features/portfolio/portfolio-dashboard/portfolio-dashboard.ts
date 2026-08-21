import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PortfolioService } from '../services/portfolio.service';
import { LeadershipEngagementListItem, StatusFilterType, DaysFilterType, ResourcingFilterType } from '../models/portfolio.model';
import {
  PORTFOLIO_STATUS,
  PORTFOLIO_DAYS_FILTER,
  PORTFOLIO_MESSAGES,
  PORTFOLIO_BADGE_CLASSES,
  APP_ROUTES,
  ENGAGEMENT_ROUTES
} from '@shared/constants/app.constants';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';
import { BadgeComponent } from '@common/components/badge/badge';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

@Component({
  selector: 'app-portfolio-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InlineSVGModule,
    AlertComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    BadgeComponent,
    LoadingSpinnerComponent,
    PageHeaderComponent,
    SelectComponent
  ],
  templateUrl: './portfolio-dashboard.html',
  styleUrl: './portfolio-dashboard.scss'
})
export class PortfolioDashboardComponent implements OnInit {
  private portfolioService = inject(PortfolioService);
  private router = inject(Router);

  readonly STATUS = PORTFOLIO_STATUS;
  readonly DAYS = PORTFOLIO_DAYS_FILTER;
  readonly MESSAGES = PORTFOLIO_MESSAGES;

  readonly statusOptions: SelectOption[] = [
    { value: PORTFOLIO_STATUS.ALL, label: 'All' },
    { value: PORTFOLIO_STATUS.KICKOFF_READY, label: 'Kickoff Ready' },
    { value: PORTFOLIO_STATUS.IN_PROGRESS, label: 'In Progress' },
    { value: PORTFOLIO_STATUS.AT_RISK, label: 'At Risk' }
  ];

  readonly daysOptions: SelectOption[] = [
    { value: PORTFOLIO_DAYS_FILTER.ALL, label: 'All' },
    { value: PORTFOLIO_DAYS_FILTER.DAYS_7, label: '≤ 7 Days' },
    { value: PORTFOLIO_DAYS_FILTER.DAYS_14, label: '≤ 14 Days' },
    { value: PORTFOLIO_DAYS_FILTER.DAYS_30, label: '≤ 30 Days' }
  ];

  readonly resourcingOptions: SelectOption[] = [
    { value: 'ALL', label: 'All' },
    { value: 'FLAGGED', label: 'Resourcing Flagged' }
  ];

  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  engagements = signal<LeadershipEngagementListItem[]>([]);

  statusFilter = signal<StatusFilterType>(PORTFOLIO_STATUS.ALL);
  daysFilter = signal<DaysFilterType>(PORTFOLIO_DAYS_FILTER.ALL);
  resourcingFilter = signal<ResourcingFilterType>('ALL');

  // Summary KPI Metrics
  totalCount = computed(() => this.engagements().length);
  kickoffReadyCount = computed(() => this.engagements().filter(e => e.status === PORTFOLIO_STATUS.KICKOFF_READY).length);
  inProgressCount = computed(() => this.engagements().filter(e => e.status === PORTFOLIO_STATUS.IN_PROGRESS).length);
  atRiskCount = computed(() => this.engagements().filter(e => e.status === PORTFOLIO_STATUS.AT_RISK).length);

  // Filtered List
  filteredEngagements = computed(() => {
    const list = this.engagements();
    const status = this.statusFilter();
    const days = this.daysFilter();
    const resourcing = this.resourcingFilter();

    return list.filter(item => {
      if (status !== PORTFOLIO_STATUS.ALL && item.status !== status) {
        return false;
      }
      if (days !== PORTFOLIO_DAYS_FILTER.ALL) {
        const maxDays = parseInt(days, 10);
        if (item.daysToKickoff > maxDays) {
          return false;
        }
      }
      if (resourcing === 'FLAGGED' && !item.hasResourcingGap) {
        return false;
      }
      return true;
    });
  });

  ngOnInit(): void {
    this.loadPortfolioData();
  }

  loadPortfolioData(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.portfolioService.getPortfolioDashboard().subscribe({
      next: (data) => {
        this.isLoading.set(false);
        this.engagements.set(data);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set(PORTFOLIO_MESSAGES.LOAD_ERROR);
      }
    });
  }

  setStatusFilter(statusVal: unknown): void {
    this.statusFilter.set((statusVal as StatusFilterType) || PORTFOLIO_STATUS.ALL);
  }

  setDaysFilter(daysVal: unknown): void {
    this.daysFilter.set((daysVal as DaysFilterType) || PORTFOLIO_DAYS_FILTER.ALL);
  }

  setResourcingFilter(resourcingVal: unknown): void {
    this.resourcingFilter.set((resourcingVal as ResourcingFilterType) || 'ALL');
  }

  openWorkspace(engagementId: number): void {
    this.router.navigate([APP_ROUTES.ENGAGEMENTS, engagementId, ENGAGEMENT_ROUTES.WORKSPACE]);
  }

  getStatusBadgeTone(status: string): StatusBadgeTone {
    if (!status) return 'neutral';
    const s = status.toLowerCase();
    if (s.includes('ready') || s.includes('completed')) return 'success';
    if (s.includes('progress') || s.includes('active')) return 'info';
    if (s.includes('risk') || s.includes('hold') || s.includes('blocked')) return 'danger';
    if (s.includes('pending') || s.includes('draft') || s.includes('needing')) return 'warning';
    return 'neutral';
  }

  getDaysLabel(daysToKickoff: number): string {
    if (daysToKickoff === 0) return PORTFOLIO_MESSAGES.TODAY;
    if (daysToKickoff < 0) return `${PORTFOLIO_MESSAGES.OVERDUE_PREFIX} (${Math.abs(daysToKickoff)}d)`;
    return `${daysToKickoff} ${PORTFOLIO_MESSAGES.DAYS_SUFFIX}`;
  }

  getDaysBadgeClass(daysToKickoff: number): string {
    if (daysToKickoff <= 0 || daysToKickoff <= 3) return PORTFOLIO_BADGE_CLASSES.DAYS_URGENT;
    if (daysToKickoff <= 7) return PORTFOLIO_BADGE_CLASSES.DAYS_WARNING;
    return PORTFOLIO_BADGE_CLASSES.DAYS_NORMAL;
  }

  getReadinessToneClass(score: number): string {
    if (score === 100) return PORTFOLIO_BADGE_CLASSES.READINESS_SUCCESS;
    if (score >= 60) return PORTFOLIO_BADGE_CLASSES.READINESS_INFO;
    if (score >= 40) return PORTFOLIO_BADGE_CLASSES.READINESS_WARNING;
    return PORTFOLIO_BADGE_CLASSES.READINESS_DANGER;
  }
}
