import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from './toast.model';

const DEFAULT_DURATION_MS = 3000;

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private nextId = 0;
  private timers = new Map<number, ReturnType<typeof setTimeout>>();

  readonly toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', durationMs = DEFAULT_DURATION_MS, actionLabel?: string): void {
    const id = ++this.nextId;
    this.toasts.update((list) => [...list, { id, message, type, actionLabel }]);

    const timer = setTimeout(() => this.dismiss(id), durationMs);
    this.timers.set(id, timer);
  }

  open(message: string, actionLabel = 'Dismiss', durationMs = 4000): void {
    this.show(message, 'info', durationMs, actionLabel);
  }

  success(message: string, durationMs?: number): void {
    this.show(message, 'success', durationMs);
  }

  error(message: string, durationMs?: number): void {
    this.show(message, 'error', durationMs);
  }

  info(message: string, durationMs?: number): void {
    this.show(message, 'info', durationMs);
  }

  warning(message: string, durationMs?: number): void {
    this.show(message, 'warning', durationMs);
  }

  dismiss(id: number): void {
    const timer = this.timers.get(id);
    if (timer) {
      clearTimeout(timer);
      this.timers.delete(id);
    }
    this.toasts.update((toasts) => toasts.filter((t) => t.id !== id));
  }

  remove(id: number): void {
    this.dismiss(id);
  }
}
