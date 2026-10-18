import { Component, HostListener, input, output } from '@angular/core';
import { ButtonComponent } from '../button/button';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss'
})
export class ConfirmDialogComponent {
  readonly title = input<string>('Please confirm');
  readonly message = input<string>('');
  readonly confirmLabel = input<string>('Confirm');
  readonly cancelLabel = input<string>('Cancel');
  readonly isBusy = input<boolean>(false);
  readonly isDestructive = input<boolean>(true);
  readonly disableBackdropClose = input<boolean>(true);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (!this.isBusy()) {
      this.cancelled.emit();
    }
  }

  onBackdropClick(): void {
    if (!this.disableBackdropClose()) {
      this.cancelled.emit();
    }
  }

  onDialogClick(event: MouseEvent): void {
    event.stopPropagation();
  }
}
