import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PortfolioService } from '../services/portfolio.service';
import { LeadershipEngagementListItem, StatusFilterType, DaysFilterType } from '../models/portfolio.model';
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
import { AvatarComponent } from '@common/components/avatar/avatar';
import { BadgeComponent } from '@common/components/badge/badge';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { TabsComponent, TabItem } from '@common/components/tabs/tabs';

@Component({
  selector: 'app-portfolio-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    InlineSVGModule,
    AlertComponent,
    EmptyStateComponent,
    StatusBadgeComponent,
    AvatarComponent,
    BadgeComponent,
    LoadingSpinnerComponent,
    TabsComponent
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

  readonly statusTabs: TabItem[] = [
    { id: PORTFOLIO_STATUS.ALL, label: 'All' },
    { id: PORTFOLIO_STATUS.KICKOFF_READY, label: 'Kickoff Ready' },
    { id: PORTFOLIO_STATUS.IN_PROGRESS, label: 'In Progress' },
    { id: PORTFOLIO_STATUS.AT_RISK, label: 'At Risk' }
  ];

  readonly daysTabs: TabItem[] = [
    { id: PORTFOLIO_DAYS_FILTER.ALL, label: 'All' },
    { id: PORTFOLIO_DAYS_FILTER.DAYS_7, label: '≤ 7 Days' },
    { id: PORTFOLIO_DAYS_FILTER.DAYS_14, label: '≤ 14 Days' },
    { id: PORTFOLIO_DAYS_FILTER.DAYS_30, label: '≤ 30 Days' }
  ];

  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  engagements = signal<LeadershipEngagementListItem[]>([]);

  statusFilter = signal<StatusFilterType>(PORTFOLIO_STATUS.ALL);
  daysFilter = signal<DaysFilterType>(PORTFOLIO_DAYS_FILTER.ALL);

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

  setStatusFilter(statusId: string): void {
    this.statusFilter.set(statusId as StatusFilterType);
  }

  setDaysFilter(daysId: string): void {
    this.daysFilter.set(daysId as DaysFilterType);
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
