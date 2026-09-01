import { Component, inject, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardBlocker } from '@shared/models/app-context.model';
import { Router, RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DASHBOARD_CONSTANTS } from '@shared/constants/home.constants';
import { NotificationService } from '@shared/services/notification.service';

@Component({
  selector: 'app-dashboard-critical-blockers',
  standalone: true,
  imports: [CommonModule, RouterModule, InlineSVGModule],
  templateUrl: './dashboard-critical-blockers.html',
  styleUrl: './dashboard-critical-blockers.scss'
})
export class DashboardCriticalBlockersComponent {
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  blockers = input.required<DashboardBlocker[]>();
  totalCount = input.required<number>();

  onBlockerClick(blocker: DashboardBlocker): void {
    this.notificationService.getNotifications(undefined, 1, 50).subscribe({
      next: (result) => {
        const notification = result.items.find(n => n.id === blocker.riskId);
        if (notification?.link) {
          this.router.navigateByUrl(notification.link);
        } else {
          this.router.navigateByUrl('/notifications');
        }
      },
      error: () => this.router.navigateByUrl('/notifications')
    });
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
