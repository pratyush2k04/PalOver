import { Component, Input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DashboardCard } from '@shared/models/app-context.model';

@Component({
  selector: 'app-dashboard-kpi-card',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './dashboard-kpi-card.component.html',
  styleUrl: './dashboard-kpi-card.component.scss'
})
export class DashboardKpiCardComponent {
  @Input({ required: true }) card!: DashboardCard;

  readonly iconPath = computed(() => {
    switch (this.card.type) {
      case 'engagement':
        return 'assets/icons/briefcase.svg';
      case 'blocker':
        return 'assets/icons/lock.svg';
      case 'handover':
        return 'assets/icons/file.svg';
      case 'draft-handover':
        return 'assets/icons/edit.svg';
      case 'complete-handover':
        return 'assets/icons/check-circle.svg';
      case 'checklist':
        return 'assets/icons/checklist.svg';
      case 'tech-queue':
        return 'assets/icons/briefcase.svg';
      case 'access-item':
        return 'assets/icons/alert-circle.svg';
      case 'escalation':
        return 'assets/icons/alert-triangle.svg';
      default:
        return 'assets/icons/info.svg';
    }
  });
  
  readonly iconClass = computed(() => {
    switch (this.card.type) {
      case 'blocker':
      case 'escalation':
        return 'text-danger';
      case 'engagement':
      case 'handover':
      case 'tech-queue':
        return 'text-primary';
      case 'checklist':
      case 'access-item':
      case 'complete-handover':
        return 'text-success';
      case 'draft-handover':
        return 'text-warning';
      default:
        return 'text-info';
    }
  });
}
