import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { ClientContactTransitionService } from '@features/engagements/services/client-contact-transition.service';
import { TransitionResponse, AddClientContactTransitionDialogData } from '@features/engagements/models/client-contact-transition.model';
import { PersonService } from '@shared/services/person.service';
import { PersonType } from '@shared/models/person.model';
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
    AlertComponent,
    InputComponent,
    SelectComponent,
  ],
  templateUrl: './add-client-contact-transition-dialog.component.html',
  styleUrl: './add-client-contact-transition-dialog.component.scss',
})
export class AddClientContactTransitionDialogComponent {
  private dialogRef = inject<DialogRef<TransitionResponse>>(DialogRef);
  private transitionService = inject(ClientContactTransitionService);
  private personService = inject(PersonService);
  private fb = inject(FormBuilder);
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
  isSubmitting = signal(false);
  errorMessage = signal<string>('');
  outgoingContactOptions = signal<SelectOption[]>([]);

  private peopleById = new Map<number, { name: string; role: string }>();

  constructor() {
    this.personService.getPeopleByEngagement(this.data.engagementId).subscribe({
      next: (people) => {
        const clients = people.filter(
          (p) => p.personType === PersonType.Client || p.personType === 'Client' || Number(p.personType) === PersonType.Client
        );
        this.outgoingContactOptions.set(
          clients.map((p) => ({ value: p.personId, label: `${p.name} - ${p.role}` }))
        );
        this.peopleById = new Map(clients.map((p) => [p.personId, { name: p.name, role: p.role }]));
      },
    });

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

  reviewAndConfirm(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
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
