import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { InputComponent } from '@common/components/form/input/input';
import { ToastService } from '@common/services/toast/toast';
import { RESOURCE_REQUEST_CONSTANTS } from '@shared/constants/app.constants';
import { ResourceAllocationItem, ResourceRequestItem } from '../../../models/resource-request.model';
import { ResourceRequestService } from '../../../services/resource-request.service';

export interface AllocateResourcesDialogData {
  engagementId: number;
  requestId: number;
  peopleNeeded: number;
  standardRoleLabel: string;
  existingAllocations: ResourceAllocationItem[];
}

@Component({
  selector: 'app-allocate-resources-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InlineSVGModule, ButtonComponent, AlertComponent, InputComponent],
  templateUrl: './allocate-resources-dialog.component.html',
  styleUrl: './allocate-resources-dialog.component.scss',
})
export class AllocateResourcesDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<ResourceRequestItem | null>>(DialogRef);
  private resourceRequestService = inject(ResourceRequestService);
  private toastService = inject(ToastService);
  readonly data = inject<AllocateResourcesDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;

  // Always exactly peopleNeeded slots — the first existingAllocations.length are pre-filled
  // by position (disabled once confirmed); the rest are empty and editable.
  readonly slots = Array.from({ length: this.data.peopleNeeded }, (_, i) => i);

  readonly namesArray = this.fb.array(
    this.slots.map((i) => {
      const existing = this.data.existingAllocations[i];
      const control = this.fb.control(existing?.name ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      });
      if (existing?.isConfirmed) {
        control.disable();
      }
      return control;
    }),
  );

  readonly namesArrayHost = this.fb.group({ names: this.namesArray });

  errorMessage = signal<string | null>(null);
  isSubmitting = signal<boolean>(false);

  slotLabel(index: number): string {
    return `${this.data.standardRoleLabel} ${index + 1}`;
  }

  onSubmit(): void {
    if (this.namesArray.invalid) {
      this.namesArray.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const names = this.namesArray.getRawValue().map((name) => name.trim());

    this.resourceRequestService
      .allocateResources(this.data.engagementId, this.data.requestId, { names })
      .subscribe({
        next: (updated: ResourceRequestItem) => {
          this.toastService.success(RESOURCE_REQUEST_CONSTANTS.MSG_SUCCESS_ALLOCATED);
          this.isSubmitting.set(false);
          this.dialogRef.close(updated);
        },
        error: (err: { error?: { message?: string; title?: string } }) => {
          this.isSubmitting.set(false);
          const errorMsg =
            err.error?.message || err.error?.title || 'Failed to save named resources. Please try again.';
          this.errorMessage.set(errorMsg);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
