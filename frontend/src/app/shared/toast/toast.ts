import { Component, inject } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './toast.html',
  styleUrl: './toast.scss'
})
export class ToastComponent {
  private toastService = inject(ToastService);

  toasts = this.toastService.toasts;

  icon(type: string): string {
    switch (type) {
      case 'success':
        return '✓';
      case 'error':
        return '✕';
      default:
        return 'ⓘ';
    }
  }

  dismiss(id: number): void {
    this.toastService.dismiss(id);
  }
}
