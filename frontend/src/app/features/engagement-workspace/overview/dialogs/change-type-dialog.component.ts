import { Component, EventEmitter, Input, Output, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface ChangeTypeResult {
  nature: string;
  commercialModel: string;
}

@Component({
  selector: 'app-change-type-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Change engagement type</h2>
          <button type="button" class="close-btn" (click)="onCancel()">&times;</button>
        </div>

        <div class="modal-body">
          <div class="form-group">
            <label for="natureSelect">Nature</label>
            <select id="natureSelect" [(ngModel)]="selectedNature" class="form-control">
              <option value="Brownfield">Brownfield</option>
              <option value="Greenfield">Greenfield</option>
            </select>
          </div>

          <div class="form-group">
            <label for="commercialModelSelect">Commercial model</label>
            <select id="commercialModelSelect" [(ngModel)]="selectedCommercialModel" class="form-control">
              <option value="Fixed">Fixed</option>
              <option value="Time & Material">Time & Material</option>
              <option value="Capped">Capped</option>
            </select>
          </div>

          <p class="modal-info-text">
            Changing the nature adds or removes the brownfield-only technical steps (code review, architecture analysis). Removed items are kept in history, never deleted.
          </p>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-outline" (click)="onCancel()">Cancel</button>
          <button type="button" class="btn btn-primary" (click)="onSave()">Save</button>
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
      z-index: 1000;
    }

    .modal-card {
      background: #ffffff;
      border-radius: 0.75rem;
      width: 100%;
      max-width: 30rem;
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

    .form-group {
      margin-bottom: 1.25rem;
    }

    .form-group label {
      display: block;
      font-size: 0.875rem;
      font-weight: 600;
      color: #374151;
      margin-bottom: 0.375rem;
    }

    .form-control {
      width: 100%;
      padding: 0.625rem 0.75rem;
      font-size: 0.875rem;
      border: 1px solid #d1d5db;
      border-radius: 0.5rem;
      background-color: #ffffff;
      color: #111827;
      outline: none;
      transition: border-color 0.15s ease-in-out;
    }

    .form-control:focus {
      border-color: #f59e0b;
      box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1);
    }

    .modal-info-text {
      font-size: 0.875rem;
      color: #4b5563;
      line-height: 1.4;
      margin-top: 1rem;
      margin-bottom: 0;
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

    .btn-primary {
      background: #f59e0b;
      border: 1px solid #f59e0b;
      color: #ffffff;
    }

    .btn-primary:hover {
      background: #d97706;
      border-color: #d97706;
    }
  `]
})
export class ChangeTypeDialogComponent implements OnInit {
  @Input() currentNature: string = 'Greenfield';
  @Input() currentCommercialModel: string = 'Fixed';

  @Output() save = new EventEmitter<ChangeTypeResult>();
  @Output() cancel = new EventEmitter<void>();

  selectedNature: string = 'Greenfield';
  selectedCommercialModel: string = 'Fixed';

  ngOnInit() {
    if (this.currentNature) {
      this.selectedNature = this.currentNature;
    }
    if (this.currentCommercialModel) {
      const modelLower = this.currentCommercialModel.toLowerCase();
      if (modelLower.includes('time') || modelLower.includes('t&m')) {
        this.selectedCommercialModel = 'Time & Material';
      } else if (modelLower.includes('capped')) {
        this.selectedCommercialModel = 'Capped';
      } else {
        this.selectedCommercialModel = 'Fixed';
      }
    }
  }

  onSave() {
    this.save.emit({
      nature: this.selectedNature,
      commercialModel: this.selectedCommercialModel
    });
  }

  onCancel() {
    this.cancel.emit();
  }
}
