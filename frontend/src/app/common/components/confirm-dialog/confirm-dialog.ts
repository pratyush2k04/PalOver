import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { ButtonComponent } from '../button/button';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss'
})
export class ConfirmDialogComponent {
  @Input() title = 'Please confirm';
  @Input() message = '';
  @Input() confirmLabel = 'Confirm';
  @Input() cancelLabel = 'Cancel';
  @Input() isBusy = false;
  @Input() isDestructive = true;
  @Input() disableBackdropClose = true;

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (!this.isBusy) {
      this.cancelled.emit();
    }
  }

  onBackdropClick(): void {
    if (!this.disableBackdropClose) {
      this.cancelled.emit();
    }
  }

  onDialogClick(event: MouseEvent): void {
    event.stopPropagation();
  }
}
