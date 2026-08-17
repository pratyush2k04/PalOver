import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DayOneBlockerItemDto } from '@features/engagements/models/technical-readiness/technical-summary.dtos';

@Component({
  selector: 'app-summary-print-blockers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './summary-print-blockers.component.html',
  styleUrl: './summary-print-blockers.component.scss'
})
export class SummaryPrintBlockersComponent {
  blockers = input.required<DayOneBlockerItemDto[]>();
}
