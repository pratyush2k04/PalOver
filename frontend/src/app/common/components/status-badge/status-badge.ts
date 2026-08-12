import { Component, input } from '@angular/core';

export type StatusBadgeTone = 'info' | 'neutral' | 'success' | 'danger' | 'warning';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.scss'
})
export class StatusBadgeComponent {
  readonly tone = input<StatusBadgeTone>('neutral');
}
