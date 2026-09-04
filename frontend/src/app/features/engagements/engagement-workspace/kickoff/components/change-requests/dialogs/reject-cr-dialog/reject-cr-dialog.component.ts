import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { ChangeRequest, RejectChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface RejectCrDialogData {
  cr: ChangeRequest;
}

@Component({
  selector: 'app-reject-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './reject-cr-dialog.component.html',
  styleUrl: './reject-cr-dialog.component.scss'
})
export class RejectCrDialogComponent {
  private dialogRef = inject<DialogRef<RejectChangeRequest>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<RejectCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  rejectForm: FormGroup = this.fb.group({
    rejectionReason: ['', [Validators.required, Validators.maxLength(500)]]
  });

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.rejectForm.invalid) {
      this.rejectForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close({
      rejectionReason: this.rejectForm.value.rejectionReason
    });
  }
}
