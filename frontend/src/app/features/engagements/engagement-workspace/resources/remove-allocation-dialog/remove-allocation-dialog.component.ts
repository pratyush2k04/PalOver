import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { ToastService } from '@common/services/toast/toast';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { ResourceRequestItem } from '../../../models/resource-request.model';
import { ResourceRequestService } from '../../../services/resource-request.service';

export interface RemoveAllocationDialogData {
  engagementId: number;
  requestId: number;
  allocationId: number;
  allocationName: string;
}

@Component({
  selector: 'app-remove-allocation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, AlertComponent, ButtonComponent, TextareaComponent],
  templateUrl: './remove-allocation-dialog.component.html',
  styleUrl: './remove-allocation-dialog.component.scss',
})
export class RemoveAllocationDialogComponent {
  private resourceRequestService = inject(ResourceRequestService);
  private toastService = inject(ToastService);
  private dialogRef = inject<DialogRef<ResourceRequestItem | null>>(DialogRef);
  readonly data = inject<RemoveAllocationDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  reason = signal<string>('');
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  submitted = signal<boolean>(false);

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
    if (this.errorMessage()) {
      this.errorMessage.set(null);
    }
  }

  onCancel(): void {
    if (!this.isSubmitting()) {
      this.dialogRef.close(null);
    }
  }

  onSubmit(): void {
    if (!this.isValidReason() || this.isSubmitting()) {
      this.submitted.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.resourceRequestService
      .removeAllocation(this.data.engagementId, this.data.requestId, this.data.allocationId, {
        reason: this.trimmedReason(),
      })
      .subscribe({
        next: (updated: ResourceRequestItem) => {
          this.toastService.success(RESOURCE_REQUEST_CONSTANTS.MSG_SUCCESS_REMOVED);
          this.isSubmitting.set(false);
          this.dialogRef.close(updated);
        },
        error: (err: { error?: { message?: string; title?: string } }) => {
          this.isSubmitting.set(false);
          const errorMsg = err.error?.message || err.error?.title || 'Failed to remove resource. Please try again.';
          this.errorMessage.set(errorMsg);
        },
      });
  }
}
