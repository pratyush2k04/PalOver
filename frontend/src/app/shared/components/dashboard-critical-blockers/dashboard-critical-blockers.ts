import { Component, input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { DashboardBlocker } from '@shared/models/app-context.model';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';

@Component({
  selector: 'app-dashboard-critical-blockers',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule, InlineSVGModule],
  templateUrl: './dashboard-critical-blockers.html',
  styleUrl: './dashboard-critical-blockers.scss'
})
export class DashboardCriticalBlockersComponent {
  blockers = input.required<DashboardBlocker[]>();

  isOverdue(dueDate: string): boolean {
    if (!dueDate) return false;
    const due = new Date(dueDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const dueTime = new Date(due);
    dueTime.setHours(0, 0, 0, 0);
    
    return dueTime < today;
  }

  isToday(dueDate: string): boolean {
    if (!dueDate) return false;
    const due = new Date(dueDate);
    const today = new Date();
    return due.toDateString() === today.toDateString();
  }
}
