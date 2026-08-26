import { Component, Input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DashboardCard } from '@shared/models/app-context.model';

const CARD_TYPE_STYLE: Record<string, { icon: string; class: string }> = {
  engagement: { icon: 'assets/icons/briefcase.svg', class: 'text-primary' },
  blocker: { icon: 'assets/icons/lock.svg', class: 'text-danger' },
  handover: { icon: 'assets/icons/file.svg', class: 'text-primary' },
  'draft-handover': { icon: 'assets/icons/edit.svg', class: 'text-warning' },
  'complete-handover': { icon: 'assets/icons/check-circle.svg', class: 'text-success' },
  checklist: { icon: 'assets/icons/checklist.svg', class: 'text-success' },
  'tech-queue': { icon: 'assets/icons/briefcase.svg', class: 'text-primary' },
  'access-item': { icon: 'assets/icons/alert-circle.svg', class: 'text-success' },
  escalation: { icon: 'assets/icons/alert-triangle.svg', class: 'text-danger' }
};
const DEFAULT_CARD_STYLE = { icon: 'assets/icons/info.svg', class: 'text-info' };

@Component({
  selector: 'app-dashboard-kpi-card',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './dashboard-kpi-card.component.html',
  styleUrl: './dashboard-kpi-card.component.scss'
})
export class DashboardKpiCardComponent {
  @Input({ required: true }) card!: DashboardCard;

  private readonly style = computed(() => CARD_TYPE_STYLE[this.card.type] ?? DEFAULT_CARD_STYLE);

  readonly iconPath = computed(() => this.style().icon);
  readonly iconClass = computed(() => this.style().class);
}
