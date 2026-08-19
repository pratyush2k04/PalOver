import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatusBadgeComponent, StatusBadgeTone } from '@common/components/status-badge/status-badge';

@Component({
  selector: 'app-summary-print-hero',
  standalone: true,
  imports: [CommonModule, StatusBadgeComponent],
  templateUrl: './summary-print-hero.component.html',
  styleUrl: './summary-print-hero.component.scss'
})
export class SummaryPrintHeroComponent {
  isAbleToStart = input.required<boolean>();
  message = input.required<string>();
  statusTone = input.required<StatusBadgeTone>();
}
