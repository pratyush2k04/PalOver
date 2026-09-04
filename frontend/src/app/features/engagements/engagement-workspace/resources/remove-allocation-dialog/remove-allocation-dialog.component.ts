import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { RemoveAllocationPayload } from '../../../models/resource-request.model';
import { ResourceRequestService } from '@features/engagements/services/resource-request.service';

export interface RemoveAllocationDialogData {
  engagementId: number;
  requestId: number;
  allocationId: number;
  allocationName: string;
}

@Component({
  selector: 'app-remove-allocation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AlertComponent, TextareaComponent],
  templateUrl: './remove-allocation-dialog.component.html',
  styleUrl: './remove-allocation-dialog.component.scss',
})
export class RemoveAllocationDialogComponent {
  private dialogRef = inject<DialogRef<RemoveAllocationPayload>>(DialogRef);
  private resourceRequestService = inject(ResourceRequestService);
  private destroyRef = inject(DestroyRef);
  readonly data = inject<RemoveAllocationDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  reason = signal<string>('');
  submitted = signal<boolean>(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  trimmedReason = computed(() => this.reason().trim());
  isValidReason = computed(() => {
    const len = this.trimmedReason().length;
    return len >= 10 && len <= 500;
  });
  minLengthError = computed(() => {
    const len = this.trimmedReason().length;
    return len < 10 && (len > 0 || this.submitted()) ? this.constants.MSG_REMOVAL_REASON_LENGTH : null;
  });

  onReasonChange(value: string): void {
    this.reason.set(value);
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.isValidReason()) {
      this.submitted.set(true);
      return;
    }

    const payload: RemoveAllocationPayload = { reason: this.trimmedReason() };

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.resourceRequestService
      .removeAllocation(this.data.engagementId, this.data.requestId, this.data.allocationId, payload, true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(payload);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(err?.error?.message || err?.error || 'Failed to remove resource. Please try again.');
        },
      });
  }
}
