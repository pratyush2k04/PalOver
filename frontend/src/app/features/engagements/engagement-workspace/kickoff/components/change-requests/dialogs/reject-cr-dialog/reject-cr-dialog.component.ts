import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface RejectCrDialogData {
  cr: ChangeRequest;
}

@Component({
  selector: 'app-reject-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AlertComponent, ButtonComponent, TextareaComponent],
  templateUrl: './reject-cr-dialog.component.html',
  styleUrl: './reject-cr-dialog.component.scss'
})
export class RejectCrDialogComponent {
  private dialogRef = inject<DialogRef<ChangeRequest>>(DialogRef);
  private changeRequestService = inject(ChangeRequestService);
  private fb = inject(FormBuilder);
  readonly data = inject<RejectCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  rejectForm: FormGroup = this.fb.group({
    rejectionReason: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      rejectionReason: this.rejectForm.value.rejectionReason
    };

    this.changeRequestService.rejectChangeRequest(this.data.cr.id, payload).subscribe({
      next: (updatedCr) => {
        this.isSubmitting.set(false);
        this.dialogRef.close(updatedCr);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
    });
  }
}
