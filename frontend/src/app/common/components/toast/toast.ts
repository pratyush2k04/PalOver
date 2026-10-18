import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { ToastService } from '../../services/toast/toast';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule, InlineSVGModule],
  templateUrl: './toast.html',
  styleUrl: './toast.scss'
})
export class ToastComponent {
  private toastService = inject(ToastService);

  toasts = this.toastService.toasts;

  readonly iconMap: Record<string, string> = {
    success: 'assets/icons/check-circle.svg',
    error: 'assets/icons/alert-circle.svg',
    warning: 'assets/icons/alert-triangle.svg',
    info: 'assets/icons/info.svg'
  };

  dismiss(id: number, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.toastService.dismiss(id);
  }
}
