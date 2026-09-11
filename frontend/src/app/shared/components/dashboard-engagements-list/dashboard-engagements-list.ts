import { Component, input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { DashboardEngagement } from '@shared/models/app-context.model';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { TooltipDirective } from '@common/directives/tooltip.directive';

@Component({
  selector: 'app-dashboard-engagements-list',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule, InlineSVGModule, EmptyStateComponent, TooltipDirective],
  templateUrl: './dashboard-engagements-list.html',
  styleUrl: './dashboard-engagements-list.scss'
})
export class DashboardEngagementsListComponent {
  engagements = input.required<DashboardEngagement[]>();
  totalCount = input.required<number>();

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
