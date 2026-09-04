import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { RemoveAllocationPayload } from '../../../models/resource-request.model';

export interface RemoveAllocationDialogData {
  engagementId: number;
  requestId: number;
  allocationId: number;
  allocationName: string;
}

@Component({
  selector: 'app-remove-allocation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './remove-allocation-dialog.component.html',
  styleUrl: './remove-allocation-dialog.component.scss',
})
export class RemoveAllocationDialogComponent {
  private dialogRef = inject<DialogRef<RemoveAllocationPayload>>(DialogRef);
  readonly data = inject<RemoveAllocationDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  reason = signal<string>('');
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
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.isValidReason()) {
      this.submitted.set(true);
      return;
    }

    this.dialogRef.close({ reason: this.trimmedReason() });
  }
}
