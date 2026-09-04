import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { CreateChangeRequest } from '@features/engagements/models/change-request.model';
import { CHANGE_REQUEST_CONSTANTS } from '@shared/constants/change-request.constants';

export interface LogCrDialogData {
  engagementId: number;
}

@Component({
  selector: 'app-log-cr-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, TextareaComponent],
  templateUrl: './log-cr-dialog.component.html',
  styleUrl: './log-cr-dialog.component.scss'
})
export class LogCrDialogComponent {
  private dialogRef = inject<DialogRef<CreateChangeRequest>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<LogCrDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  logForm: FormGroup = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(500)]],
    impact: ['', [Validators.required, Validators.maxLength(500)]]
  });

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.logForm.invalid) {
      this.logForm.markAllAsTouched();
      return;
    }

    this.dialogRef.close({
      description: this.logForm.value.description,
      impact: this.logForm.value.impact
    });
  }
}
