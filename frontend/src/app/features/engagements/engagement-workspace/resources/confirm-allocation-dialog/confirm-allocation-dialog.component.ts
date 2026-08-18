import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { InputComponent } from '@common/components/form/input/input';
import { ToastService } from '@common/services/toast/toast';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { ResourceRequestItem } from '../../../models/resource-request.model';
import { ResourceRequestService } from '../../../services/resource-request.service';

export interface ConfirmAllocationDialogData {
  engagementId: number;
  requestId: number;
  allocationId: number;
  allocationName: string;
}

function notFutureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }
    const todayStr = new Date().toISOString().split('T')[0];
    return control.value > todayStr ? { futureDate: true } : null;
  };
}

@Component({
  selector: 'app-confirm-allocation-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InlineSVGModule, ButtonComponent, AlertComponent, InputComponent],
  templateUrl: './confirm-allocation-dialog.component.html',
  styleUrl: './confirm-allocation-dialog.component.scss',
})
export class ConfirmAllocationDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<ResourceRequestItem | null>>(DialogRef);
  private resourceRequestService = inject(ResourceRequestService);
  private toastService = inject(ToastService);
  readonly data = inject<ConfirmAllocationDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  readonly form = this.fb.nonNullable.group({
    confirmedDate: [new Date().toISOString().split('T')[0], [notFutureDateValidator()]],
  });

  errorMessage = signal<string | null>(null);
  isSubmitting = signal<boolean>(false);

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const { confirmedDate } = this.form.getRawValue();

    this.resourceRequestService
      .confirmAllocation(this.data.engagementId, this.data.requestId, this.data.allocationId, { confirmedDate })
      .subscribe({
        next: (updated: ResourceRequestItem) => {
          this.toastService.success(RESOURCE_REQUEST_CONSTANTS.MSG_SUCCESS_CONFIRMED);
          this.isSubmitting.set(false);
          this.dialogRef.close(updated);
        },
        error: (err: { error?: { message?: string; title?: string } }) => {
          this.isSubmitting.set(false);
          const errorMsg = err.error?.message || err.error?.title || 'Failed to confirm resource. Please try again.';
          this.errorMessage.set(errorMsg);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
