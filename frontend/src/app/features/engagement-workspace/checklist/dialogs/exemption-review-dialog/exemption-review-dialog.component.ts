import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '../../../../../shared/dialog/dialog-ref';
import { DIALOG_DATA } from '../../../../../shared/dialog/dialog.service';

export interface ExemptionReviewDialogData {
  itemTitle: string;
  reason: string;
  requestedAt?: string | null;
}

export interface ExemptionReviewDialogResult {
  approved: boolean;
  rejectionReason?: string;
}

@Component({
  selector: 'app-exemption-review-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './exemption-review-dialog.component.html',
  styleUrl: './exemption-review-dialog.component.scss'
})
export class ExemptionReviewDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<ExemptionReviewDialogResult>>(DialogRef);
  readonly data = inject<ExemptionReviewDialogData>(DIALOG_DATA);

  isRejectMode = signal<boolean>(false);

  readonly form = this.fb.nonNullable.group({
    rejectionReason: ['', [Validators.maxLength(500)]]
  });

  get rejectionReasonControl() {
    return this.form.get('rejectionReason')!;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onApprove(): void {
    this.dialogRef.close({ approved: true });
  }

  onShowRejectMode(): void {
    this.isRejectMode.set(true);
  }

  onConfirmReject(): void {
    const rejectionReason = this.rejectionReasonControl.value?.trim() || undefined;
    this.dialogRef.close({ approved: false, rejectionReason });
  }
}
