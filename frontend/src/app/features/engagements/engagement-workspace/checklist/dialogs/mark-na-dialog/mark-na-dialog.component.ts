import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface MarkNaDialogData {
  itemTitle: string;
  isMandatory: boolean;
}

export interface MarkNaDialogResult {
  reason: string;
}

@Component({
  selector: 'app-mark-na-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './mark-na-dialog.component.html',
  styleUrl: './mark-na-dialog.component.scss',
})
export class MarkNaDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<MarkNaDialogResult>>(DialogRef);
  readonly data = inject<MarkNaDialogData>(DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({
    reason: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]],
  });

  get reasonControl() {
    return this.form.get('reason')!;
  }

  get reasonLength(): number {
    return this.reasonControl.value?.length || 0;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const reason = this.reasonControl.value.trim();
    this.dialogRef.close({ reason });
  }
}
