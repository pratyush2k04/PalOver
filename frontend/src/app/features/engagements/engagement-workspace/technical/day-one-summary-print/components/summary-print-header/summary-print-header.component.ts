import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DayOneReadinessSummaryDto } from '@features/engagements/models/technical-readiness/technical-summary.dtos';

@Component({
  selector: 'app-summary-print-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './summary-print-header.component.html',
  styleUrl: './summary-print-header.component.scss'
})
export class SummaryPrintHeaderComponent {
  summary = input.required<DayOneReadinessSummaryDto>();
}
