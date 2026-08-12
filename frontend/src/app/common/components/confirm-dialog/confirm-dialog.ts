import { Component, EventEmitter, Input, Output } from '@angular/core';
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

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  onBackdropClick(): void {
    this.cancelled.emit();
  }

  onDialogClick(event: MouseEvent): void {
    event.stopPropagation();
  }
}
