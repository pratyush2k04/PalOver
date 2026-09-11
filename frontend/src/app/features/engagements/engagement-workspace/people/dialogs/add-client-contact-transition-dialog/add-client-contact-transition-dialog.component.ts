import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { AlertComponent } from '@common/components/alert/alert';
import { AddClientContactTransitionDialogData, AddTransitionRequest } from '@features/engagements/models/client-contact-transition.model';
import { ClientContactTransitionService } from '@features/engagements/services/client-contact-transition.service';
import { PersonType } from '@shared/models/person.model';
import { ToastService } from '@common/services/toast/toast';
import {
  FORM_LABELS,
  FORM_PLACEHOLDERS,
  BUTTON_LABELS,
  DIALOG_LABELS,
  VALIDATION_MESSAGES,
  STATUS_MESSAGES,
} from '@shared/constants/client-contact-transition.constants';

@Component({
  selector: 'app-add-client-contact-transition-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    InputComponent,
    SelectComponent,
    AlertComponent,
  ],
  templateUrl: './add-client-contact-transition-dialog.component.html',
  styleUrl: './add-client-contact-transition-dialog.component.scss',
})
export class AddClientContactTransitionDialogComponent {
  private dialogRef = inject<DialogRef<boolean>>(DialogRef);
  private transitionService = inject(ClientContactTransitionService);
  private fb = inject(FormBuilder);
  private toastService = inject(ToastService);
  readonly data = inject<AddClientContactTransitionDialogData>(DIALOG_DATA);

  formLabels = FORM_LABELS;
  formPlaceholders = FORM_PLACEHOLDERS;
  buttonLabels = BUTTON_LABELS;
  dialogLabels = DIALOG_LABELS;
  validationMessages = VALIDATION_MESSAGES;
  statusMessages = STATUS_MESSAGES;

  form: FormGroup = this.fb.group({
    outgoingPersonId: [null as number | null, Validators.required],
    outgoingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    outgoingContactRole: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactName: ['', [Validators.required, Validators.maxLength(200)]],
    incomingContactRole: ['', [Validators.required, Validators.maxLength(200)]],
  });

  showConfirm = signal(false);
  outgoingContactOptions = signal<SelectOption[]>([]);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  private peopleById = new Map<number, { name: string; role: string }>();

  constructor() {
    const outgoingNames = new Set(this.data.activeTransitions.map(t => t.outgoingContactName));
    const clients = this.data.people.filter(
      (p) => (p.personType === PersonType.Client || p.personType === 'Client' || Number(p.personType) === PersonType.Client)
             && !outgoingNames.has(p.name)
    );
    this.outgoingContactOptions.set(
      clients.map((p) => ({ value: p.personId, label: `${p.name} - ${p.role}` }))
    );
    this.peopleById = new Map(clients.map((p) => [p.personId, { name: p.name, role: p.role }]));

    this.form.get('outgoingPersonId')!.valueChanges.subscribe((personId: number | null) => {
      const person = personId !== null ? this.peopleById.get(personId) : undefined;
      this.form.patchValue({
        outgoingContactName: person?.name ?? '',
        outgoingContactRole: person?.role ?? '',
      });
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onOutgoingSelectClick(): void {
    if (this.outgoingContactOptions().length === 0) {
      this.toastService.info(this.validationMessages.NO_OUTGOING_CONTACTS);
    }
  }

  reviewAndConfirm(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }
    this.showConfirm.set(true);
  }

  cancelConfirm(): void {
    this.showConfirm.set(false);
  }

  confirmAndSave(): void {
    if (!this.form.valid) return;

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    const request: AddTransitionRequest = this.form.getRawValue();
    this.transitionService.saveTransition(this.data.engagementId, request, true).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err: any) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || err?.error || this.statusMessages.SAVE_ERROR);
      },
    });
  }
}
