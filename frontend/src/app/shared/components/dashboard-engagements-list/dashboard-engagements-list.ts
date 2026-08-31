import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { DashboardEngagement } from '@shared/models/app-context.model';
import { DashboardEngagementsService } from '@shared/services/dashboard-engagements.service';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { PaginatorComponent } from '@common/components/paginator/paginator';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-dashboard-engagements-list',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule, InlineSVGModule, PaginatorComponent, LoadingSpinnerComponent],
  templateUrl: './dashboard-engagements-list.html',
  styleUrl: './dashboard-engagements-list.scss'
})
export class DashboardEngagementsListComponent implements OnInit {
  private dashboardEngagementsService = inject(DashboardEngagementsService);

  readonly pageSize = 5;
  page = signal(1);
  totalCount = signal(0);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));
  pagedEngagements = signal<DashboardEngagement[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    this.loadPage();
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }
    this.page.set(page);
    this.loadPage();
  }

  private loadPage(): void {
    this.isLoading.set(true);
    this.dashboardEngagementsService.getPage(this.page(), this.pageSize).subscribe({
      next: (result) => {
        this.pagedEngagements.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      }
    });
  }

  getStatusClass(status: string): string {
    switch(status.toLowerCase()) {
      case 'kickoff-ready': return 'status-ready';
      case 'at risk': return 'status-at-risk';
      case 'in progress': return 'status-in-progress';
      default: return 'status-default';
    }
  }

  getStatusIconClass(status: string): string {
    switch(status.toLowerCase()) {
      case 'kickoff-ready': return 'icon-ready';
      case 'at risk': return 'icon-at-risk';
      case 'in progress': return 'icon-in-progress';
      default: return 'icon-default';
    }
  }

  getStatusFillClass(status: string): string {
    switch(status.toLowerCase()) {
      case 'kickoff-ready': return 'fill-ready';
      case 'at risk': return 'fill-at-risk';
      case 'in progress': return 'fill-in-progress';
      default: return 'fill-default';
    }
  }

  getDaysToGoClass(days: number): string {
    if (days < 0) return 'text-overdue';
    if (days <= 7) return 'text-warning';
    return 'text-normal';
  }
}
