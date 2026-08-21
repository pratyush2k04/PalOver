import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { AssumptionDto } from '../../../../models/assumption.model';

export interface AssumptionFormDialogData {
  assumption: AssumptionDto | null;
}

export interface AssumptionFormDialogResult {
  description: string;
  rationale: string;
}

@Component({
  selector: 'app-assumption-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './assumption-form-dialog.component.html',
  styleUrl: './assumption-form-dialog.component.scss'
})
export class AssumptionFormDialogComponent {
  private dialogRef = inject<DialogRef<AssumptionFormDialogResult>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<AssumptionFormDialogData>(DIALOG_DATA);

  form = this.fb.group({
    description: ['', [Validators.required]],
    rationale: ['', [Validators.required]]
  });

  get isEditing(): boolean {
    return !!this.data.assumption;
  }

  constructor() {
    if (this.data.assumption) {
      this.form.patchValue({
        description: this.data.assumption.description,
        rationale: this.data.assumption.rationale
      });
      this.form.markAsPristine();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditing && !this.form.dirty) {
      this.dialogRef.close();
      return;
    }

    this.dialogRef.close({
      description: this.form.value.description!.trim(),
      rationale: this.form.value.rationale!.trim()
    });
  }
}
