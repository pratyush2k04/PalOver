import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from '../models/toast.model';

const DEFAULT_DURATION_MS = 3000;

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private nextId = 0;
  readonly toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', durationMs = DEFAULT_DURATION_MS): void {
    const id = this.nextId++;
    this.toasts.update((list) => [...list, { id, message, type }]);

    setTimeout(() => this.dismiss(id), durationMs);
  }

  success(message: string, durationMs?: number): void {
    this.show(message, 'success', durationMs);
  }

  error(message: string, durationMs?: number): void {
    this.show(message, 'error', durationMs);
  }

  dismiss(id: number): void {
    this.toasts.update((list) => list.filter((t) => t.id !== id));
  }
}
