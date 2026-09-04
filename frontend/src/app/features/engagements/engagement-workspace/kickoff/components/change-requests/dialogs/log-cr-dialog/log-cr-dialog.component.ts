import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { ChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface LogCrDialogData {
  engagementId: number;
}

@Component({
  selector: 'app-log-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AlertComponent, ButtonComponent, InputComponent, TextareaComponent],
  templateUrl: './log-cr-dialog.component.html',
  styleUrl: './log-cr-dialog.component.scss'
})
export class LogCrDialogComponent {
  private dialogRef = inject<DialogRef<ChangeRequest>>(DialogRef);
  private changeRequestService = inject(ChangeRequestService);
  private fb = inject(FormBuilder);
  readonly data = inject<LogCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  logForm: FormGroup = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
    impact: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(2000)]]
  });

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.logForm.invalid) {
      this.logForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      description: this.logForm.value.description,
      impact: this.logForm.value.impact
    };

    this.changeRequestService.createChangeRequest(this.data.engagementId, payload).subscribe({
      next: (newCr) => {
        this.isSubmitting.set(false);
        this.dialogRef.close(newCr);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
    });
  }
}
