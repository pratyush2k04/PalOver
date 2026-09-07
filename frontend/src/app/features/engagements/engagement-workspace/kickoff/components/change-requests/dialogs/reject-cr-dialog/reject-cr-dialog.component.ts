import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { AlertComponent } from '@common/components/alert/alert';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface RejectCrDialogData {
  cr: ChangeRequest;
}

@Component({
  selector: 'app-reject-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent, AlertComponent],
  templateUrl: './reject-cr-dialog.component.html',
  styleUrl: './reject-cr-dialog.component.scss'
})
export class RejectCrDialogComponent {
  private dialogRef = inject<DialogRef<boolean>>(DialogRef);
  private fb = inject(FormBuilder);
  private changeRequestService = inject(ChangeRequestService);
  readonly data = inject<RejectCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  rejectForm: FormGroup = this.fb.group({
    rejectionReason: ['', [Validators.required, Validators.maxLength(500)]]
  });

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  onCancel(): void {
    if (this.isSubmitting()) return;
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    const request = {
      rejectionReason: this.rejectForm.value.rejectionReason
    };

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.changeRequestService.rejectChangeRequest(this.data.cr.id, request, true).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || err?.error || this.constants.ERROR_GENERIC);
      }
    });
  }
}
