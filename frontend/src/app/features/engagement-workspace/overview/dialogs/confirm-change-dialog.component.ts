import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm-change-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Please confirm</h2>
          <button type="button" class="close-btn" (click)="onCancel()">&times;</button>
        </div>

        <div class="modal-body">
          <p class="modal-confirm-text">
            Changing to {{ targetNature }} will add/remove 2 technical item(s). Removed items are kept in history. Continue?
          </p>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-outline" (click)="onCancel()">Cancel</button>
          <button type="button" class="btn btn-danger" (click)="onConfirm()">Confirm</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: rgba(0, 0, 0, 0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1100;
    }

    .modal-card {
      background: #ffffff;
      border-radius: 0.75rem;
      width: 100%;
      max-width: 28rem;
      box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
      overflow: hidden;
    }

    .modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1.25rem 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .modal-header h2 {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
      color: #111827;
    }

    .close-btn {
      background: transparent;
      border: none;
      font-size: 1.5rem;
      color: #6b7280;
      cursor: pointer;
      line-height: 1;
    }

    .modal-body {
      padding: 1.5rem;
    }

    .modal-confirm-text {
      font-size: 0.875rem;
      color: #374151;
      line-height: 1.5;
      margin: 0;
    }

    .modal-footer {
      display: flex;
      align-items: center;
      justify-content: flex-end;
      gap: 0.75rem;
      padding: 1rem 1.5rem;
      border-top: 1px solid #f3f4f6;
    }

    .btn {
      padding: 0.5rem 1.25rem;
      font-size: 0.875rem;
      font-weight: 500;
      border-radius: 0.5rem;
      cursor: pointer;
      transition: all 0.15s ease;
    }

    .btn-outline {
      background: #ffffff;
      border: 1px solid #d1d5db;
      color: #374151;
    }

    .btn-outline:hover {
      background: #f9fafb;
    }

    .btn-danger {
      background: #fee2e2;
      border: 1px solid #fca5a5;
      color: #dc2626;
    }

    .btn-danger:hover {
      background: #fca5a5;
    }
  `]
})
export class ConfirmChangeDialogComponent {
  @Input() targetNature: string = 'Greenfield';

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
}
