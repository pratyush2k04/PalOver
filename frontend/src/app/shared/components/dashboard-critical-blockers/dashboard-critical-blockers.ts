import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardBlocker } from '@shared/models/app-context.model';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-dashboard-critical-blockers',
  standalone: true,
  imports: [CommonModule, RouterModule, InlineSVGModule],
  templateUrl: './dashboard-critical-blockers.html',
  styleUrl: './dashboard-critical-blockers.scss'
})
export class DashboardCriticalBlockersComponent {
  blockers = input.required<DashboardBlocker[]>();

  getCategory(title: string): string {
    if (!title) return 'TASK';
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

    if (diffInSeconds < 60) return 'Just now';
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
    return `${Math.floor(diffInSeconds / 86400)}d ago`;
  }
}
