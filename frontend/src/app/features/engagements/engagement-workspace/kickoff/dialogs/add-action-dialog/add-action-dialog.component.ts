import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { KickoffActionItem } from '@shared/models/kickoff.model';
import { KICKOFF_MESSAGES } from '@shared/constants/kickoff.constants';
import { futureDateValidator } from '@common/validators/date.validators';

export type AddActionDialogResult = Pick<KickoffActionItem, 'title' | 'ownerName' | 'dueDate'>;

@Component({
  selector: 'app-add-action-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, DatepickerComponent],
  templateUrl: './add-action-dialog.component.html',
  styleUrl: './add-action-dialog.component.scss'
})
export class AddActionDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<AddActionDialogResult>>(DialogRef);

  readonly messages = KICKOFF_MESSAGES;
  readonly minDate = new Date().toISOString().split('T')[0];

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required]],
    ownerName: ['', [Validators.required]],
    dueDate: ['', [Validators.required, futureDateValidator]]
  });

  get titleControl() {
    return this.form.get('title')!;
  }

  get ownerNameControl() {
    return this.form.get('ownerName')!;
  }

  get dueDateControl() {
    return this.form.get('dueDate')!;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }
}
