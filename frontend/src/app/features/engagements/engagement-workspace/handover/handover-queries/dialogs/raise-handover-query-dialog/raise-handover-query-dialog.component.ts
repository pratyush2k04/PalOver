import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { HandoverQueryService } from '../../../../../services/handover-query.service';
import { CommitmentDto } from '../../../../../models/handover.model';
import { HandoverQueryDto, HandoverQueryFieldSection } from '../../../../../models/handover-query.model';
import { HANDOVER_MESSAGES } from '@shared/constants/handover.constants';

export interface RaiseHandoverQueryDialogData {
  engagementId: number;
  commitments: CommitmentDto[];
}

const FIELD_SECTION_OPTIONS: SelectOption[] = [
  { value: null, label: 'Select a field...' },
  { value: HandoverQueryFieldSection.ClientBackground, label: 'Client Background' },
  { value: HandoverQueryFieldSection.BusinessProblem, label: 'Business Problem' },
  { value: HandoverQueryFieldSection.ClientSentiment, label: 'Client Sentiment' },
  { value: HandoverQueryFieldSection.DealOrigin, label: 'Deal Origin' },
  { value: HandoverQueryFieldSection.Commitment, label: 'Commitment' },
];

@Component({
  selector: 'app-raise-handover-query-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, AlertComponent, SelectComponent, TextareaComponent],
  templateUrl: './raise-handover-query-dialog.component.html',
  styleUrl: './raise-handover-query-dialog.component.scss',
})
export class RaiseHandoverQueryDialogComponent {
  private dialogRef = inject<DialogRef<HandoverQueryDto>>(DialogRef);
  private handoverQueryService = inject(HandoverQueryService);
  private fb = inject(FormBuilder);
  readonly data = inject<RaiseHandoverQueryDialogData>(DIALOG_DATA);

  readonly fieldSectionOptions = FIELD_SECTION_OPTIONS;
  readonly commitmentOptions = computed<SelectOption[]>(() => [
    { value: null, label: 'Select a commitment...' },
    ...this.data.commitments.map((c) => ({ value: c.commitmentId, label: c.title })),
  ]);

  isCommitmentSection = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal('');

  form: FormGroup = this.fb.group({
    fieldSection: [null, [Validators.required]],
    referenceId: [null],
    question: ['', [Validators.required, Validators.maxLength(500)]],
  });

  readonly fieldErrors = { required: 'Please select a field.' };
  readonly commitmentErrors = { required: 'Please select a commitment.' };
  readonly questionErrors = {
    required: 'Please enter a question.',
    maxlength: 'Question cannot exceed 500 characters.',
  };

  constructor() {
    this.form.get('fieldSection')?.valueChanges.subscribe((value: HandoverQueryFieldSection | null) => {
      const isCommitment = value === HandoverQueryFieldSection.Commitment;
      this.isCommitmentSection.set(isCommitment);

      const refControl = this.form.get('referenceId');
      if (isCommitment) {
        refControl?.setValidators([Validators.required]);
      } else {
        refControl?.clearValidators();
        refControl?.setValue(null);
      }
      refControl?.updateValueAndValidity();
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const formVal = this.form.getRawValue();

    this.handoverQueryService
      .raiseQuery(this.data.engagementId, {
        fieldSection: formVal.fieldSection!,
        referenceId: formVal.referenceId ?? undefined,
        question: formVal.question.trim(),
      })
      .subscribe({
        next: (created) => {
          this.isSubmitting.set(false);
          this.dialogRef.close(created);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          const msg = (err as { error?: { message?: string } })?.error?.message;
          this.errorMessage.set(msg || HANDOVER_MESSAGES.QUERY_RAISE_ERROR);
        },
      });
  }
}
