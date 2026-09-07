import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { AlertComponent } from '@common/components/alert/alert';
import { ChangeRequestService } from '@features/engagements/services/change-request.service';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface LogCrDialogData {
  engagementId: number;
}

@Component({
  selector: 'app-log-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, TextareaComponent, AlertComponent],
  templateUrl: './log-cr-dialog.component.html',
  styleUrl: './log-cr-dialog.component.scss'
})
export class LogCrDialogComponent {
  private dialogRef = inject<DialogRef<boolean>>(DialogRef);
  private fb = inject(FormBuilder);
  private changeRequestService = inject(ChangeRequestService);
  readonly data = inject<LogCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  logForm: FormGroup = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
    impact: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(2000)]]
  });

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  onCancel(): void {
    if (this.isSubmitting()) return;
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.logForm.invalid) {
      this.logForm.markAllAsTouched();
      return;
    }

    const request = {
      description: this.logForm.value.description,
      impact: this.logForm.value.impact
    };

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.changeRequestService.createChangeRequest(this.data.engagementId, request, true).subscribe({
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
