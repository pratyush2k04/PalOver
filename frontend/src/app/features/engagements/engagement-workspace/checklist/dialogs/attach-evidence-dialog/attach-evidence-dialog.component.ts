import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { EngagementChecklistItem } from '@features/engagements/models/engagement-checklist.model';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { InputComponent } from '@common/components/form/input/input';

export interface AttachEvidenceDialogData {
  item: EngagementChecklistItem;
  isMandatoryCompletion?: boolean;
  isReadOnly?: boolean;
}

export interface AttachEvidenceDialogResult {
  note: string;
  link?: string;
  markComplete?: boolean;
}

// Simple URL validator for http/https URLs or local path/links
function urlValidator(control: { value: string | null }) {
  if (!control.value || control.value.trim() === '') {
    return null;
  }
  const val = control.value.trim();
  try {
    const url = new URL(val);
    if (url.protocol === 'http:' || url.protocol === 'https:') {
      return null;
    }
    return { invalidUrl: true };
  } catch {
    return { invalidUrl: true };
  }
}

@Component({
  selector: 'app-attach-evidence-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent, InputComponent],
  templateUrl: './attach-evidence-dialog.component.html',
  styleUrl: './attach-evidence-dialog.component.scss'
})
export class AttachEvidenceDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<AttachEvidenceDialogResult>>(DialogRef);
  readonly data = inject<AttachEvidenceDialogData>(DIALOG_DATA);

  readonly isReadOnly = this.data.isReadOnly ?? false;
  readonly isMandatoryCompletion = this.data.isMandatoryCompletion ?? false;

  readonly form = this.fb.group({
    note: [
      { value: this.data.item.evidenceNote || '', disabled: this.isReadOnly },
      [Validators.required, Validators.maxLength(500)]
    ],
    link: [
      { value: this.data.item.evidenceLink || '', disabled: this.isReadOnly },
      [Validators.maxLength(2048), urlValidator]
    ]
  });

  get noteControl() {
    return this.form.get('note')!;
  }

  get linkControl() {
    return this.form.get('link')!;
  }

  get dialogTitle(): string {
    if (this.isReadOnly) {
      return 'Evidence Details';
    }
    if (this.isMandatoryCompletion) {
      return 'Evidence required to complete this item';
    }
    return this.data.item.evidenceNote || this.data.item.evidenceLink ? 'Edit Evidence' : 'Attach Evidence';
  }

  get submitButtonText(): string {
    if (this.isMandatoryCompletion) {
      return 'Save & Complete';
    }
    return 'Save Evidence';
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.isReadOnly) {
      this.dialogRef.close();
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const note = (this.noteControl.value || '').trim();
    const linkVal = (this.linkControl.value || '').trim();
    const link = linkVal.length > 0 ? linkVal : undefined;
    const markComplete = this.isMandatoryCompletion ? true : undefined;

    this.dialogRef.close({ note, link, markComplete });
  }
}
