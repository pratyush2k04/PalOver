import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { InputComponent } from '@common/components/form/input/input';
import { ClientContactTransitionService } from '@features/engagements/services/client-contact-transition.service';
import { TransitionResponse } from '@features/engagements/models/client-contact-transition.model';
import {
  FORM_LABELS,
  FORM_PLACEHOLDERS,
  BUTTON_LABELS,
  DIALOG_LABELS,
  VALIDATION_MESSAGES,
  STATUS_MESSAGES,
} from '@shared/constants/client-contact-transition.constants';

export interface AddClientContactTransitionDialogData {
  engagementId: number;
  outgoingContactName: string;
  outgoingContactRole: string;
}

@Component({
  selector: 'app-add-client-contact-transition-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    AlertComponent,
    InputComponent,
  ],
  templateUrl: './add-client-contact-transition-dialog.component.html',
  styleUrl: './add-client-contact-transition-dialog.component.scss',
})
export class AddClientContactTransitionDialogComponent {
  private dialogRef = inject<DialogRef<TransitionResponse>>(DialogRef);
  private transitionService = inject(ClientContactTransitionService);
  private fb = inject(FormBuilder);
  readonly data = inject<AddClientContactTransitionDialogData>(DIALOG_DATA);

  formLabels = FORM_LABELS;
  formPlaceholders = FORM_PLACEHOLDERS;
  buttonLabels = BUTTON_LABELS;
  dialogLabels = DIALOG_LABELS;
  validationMessages = VALIDATION_MESSAGES;
  statusMessages = STATUS_MESSAGES;

  form: FormGroup = this.fb.group({
    outgoingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    outgoingContactRole: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactRole: ['', [Validators.required, Validators.maxLength(200)]],
  });

  showConfirm = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string>('');

  constructor() {
    this.form.patchValue({
      outgoingContactName: this.data.outgoingContactName,
      outgoingContactRole: this.data.outgoingContactRole,
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  reviewAndConfirm(): void {
    if (!this.form.valid) {
      this.errorMessage.set(this.validationMessages.REQUIRED);
      return;
    }
    this.showConfirm.set(true);
    this.errorMessage.set('');
  }

  cancelConfirm(): void {
    this.showConfirm.set(false);
  }

  confirmAndSave(): void {
    if (!this.form.valid) return;

    this.isSubmitting.set(true);
    this.errorMessage.set('');
    const data = this.form.getRawValue();

    this.transitionService.saveTransition(this.data.engagementId, data).subscribe({
      next: (response: TransitionResponse) => {
        this.isSubmitting.set(false);
        this.dialogRef.close(response);
      },
      error: (err: any) => {
        this.isSubmitting.set(false);
        const msg = err?.error?.message || this.statusMessages.SAVE_ERROR;
        this.errorMessage.set(msg);
      },
    });
  }
}
