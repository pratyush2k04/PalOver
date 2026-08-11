import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';

export interface OverrideKickoffReadyDialogData {
  engagementName: string;
  score: number;
}

export interface OverrideKickoffReadyDialogResult {
  reason: string;
}

@Component({
  selector: 'app-override-kickoff-ready-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent],
  templateUrl: './override-kickoff-ready-dialog.component.html',
  styleUrl: './override-kickoff-ready-dialog.component.scss'
})
export class OverrideKickoffReadyDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<OverrideKickoffReadyDialogResult>>(DialogRef);
  readonly data = inject<OverrideKickoffReadyDialogData>(DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({
    reason: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]]
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
