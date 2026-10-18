import { Component, input } from '@angular/core';

export type ButtonVariant = 'flat' | 'stroked' | 'accent' | 'danger' | 'icon' | 'success' | 'warning';
export type ButtonSize = 'default' | 'sm';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './button.html',
  styleUrl: './button.scss',
  host: {
    '[class.full-width]': 'fullWidth()'
  }
})
export class ButtonComponent {
  readonly variant = input<ButtonVariant>('flat');
  readonly size = input<ButtonSize>('default');
  readonly type = input<'button' | 'submit'>('button');
  readonly disabled = input(false);
  readonly loading = input(false);
  readonly fullWidth = input(false);
  readonly ariaLabel = input<string | null>(null);
  readonly title = input<string | null>(null);
}
