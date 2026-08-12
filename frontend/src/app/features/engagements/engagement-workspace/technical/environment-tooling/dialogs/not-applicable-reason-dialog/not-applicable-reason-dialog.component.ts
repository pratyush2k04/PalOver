import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface NotApplicableReasonDialogData {
  itemName: string;
  currentReason?: string;
}

export interface NotApplicableReasonDialogResult {
  reason: string;
}

@Component({
  selector: 'app-not-applicable-reason-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './not-applicable-reason-dialog.component.html',
  styleUrl: './not-applicable-reason-dialog.component.scss'
})
export class NotApplicableReasonDialogComponent {
  private dialogRef = inject<DialogRef<NotApplicableReasonDialogResult>>(DialogRef);
  readonly data = inject<NotApplicableReasonDialogData>(DIALOG_DATA);

  reason = signal(this.data.currentReason || '');
  reasonError = signal('');

  onReasonChange(value: string): void {
    this.reason.set(value);
    this.reasonError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const reason = this.reason().trim();
    if (!reason) {
      this.reasonError.set('Reason is required for Not Applicable status');
      return;
    }

    this.dialogRef.close({ reason });
  }
}
