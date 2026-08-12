import { Component, input } from '@angular/core';

export type AlertVariant = 'danger' | 'warning' | 'neutral';

@Component({
  selector: 'app-alert',
  standalone: true,
  templateUrl: './alert.html',
  styleUrl: './alert.scss'
})
export class AlertComponent {
  readonly variant = input<AlertVariant>('danger');
}
