import { Component, inject, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardBlocker } from '@shared/models/app-context.model';
import { Router, RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DASHBOARD_CONSTANTS } from '@shared/constants/home.constants';
import { NotificationService } from '@shared/services/notification.service';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { TooltipDirective } from '@common/directives/tooltip.directive';

@Component({
  selector: 'app-dashboard-critical-blockers',
  standalone: true,
  imports: [CommonModule, RouterModule, InlineSVGModule, EmptyStateComponent, TooltipDirective],
  templateUrl: './dashboard-critical-blockers.html',
  styleUrl: './dashboard-critical-blockers.scss'
})
export class DashboardCriticalBlockersComponent {
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  blockers = input.required<DashboardBlocker[]>();
  totalCount = input.required<number>();

  onBlockerClick(blocker: DashboardBlocker): void {
    this.notificationService.markAsRead(blocker.riskId).subscribe();
    this.router.navigateByUrl(blocker.link ?? '/notifications');
  }

  getCategory(title: string): string {
    if (!title) return DASHBOARD_CONSTANTS.FALLBACK_TASK;
    const parts = title.split(/ — |: /);
    return parts[0];
  }

  getEngagementName(title: string): string {
    if (!title) return '';
    const parts = title.split(/ — |: /);
    return parts.length > 1 ? parts[1] : title;
  }

  getRelativeTime(dateString: string | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diffInSeconds < 60) return DASHBOARD_CONSTANTS.TIME.JUST_NOW;
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}${DASHBOARD_CONSTANTS.TIME.MINUTES_AGO}`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}${DASHBOARD_CONSTANTS.TIME.HOURS_AGO}`;
    return `${Math.floor(diffInSeconds / 86400)}${DASHBOARD_CONSTANTS.TIME.DAYS_AGO}`;
  }
}
