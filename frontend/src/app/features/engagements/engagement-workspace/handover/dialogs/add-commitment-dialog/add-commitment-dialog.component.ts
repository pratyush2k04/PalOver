import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { HandoverService } from '../../../../services/handover.service';
import { CommitmentDto, CreateCommitmentRequest, AddCommitmentDialogData } from '../../../../models/handover.model';
import {
  HANDOVER_LABELS,
  HANDOVER_PLACEHOLDERS,
  HANDOVER_BUTTONS,
  HANDOVER_MESSAGES,
  HANDOVER_TIMELINE_UNITS,
} from '@shared/constants/handover.constants';

@Component({
  selector: 'app-add-commitment-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    AlertComponent,
    InputComponent,
    TextareaComponent,
    SelectComponent,
  ],
  templateUrl: './add-commitment-dialog.component.html',
  styleUrl: './add-commitment-dialog.component.scss',
})
export class AddCommitmentDialogComponent {
  private dialogRef = inject<DialogRef<CommitmentDto>>(DialogRef);
  private handoverService = inject(HandoverService);
  private fb = inject(FormBuilder);
  readonly data = inject<AddCommitmentDialogData>(DIALOG_DATA);

  HANDOVER_LABELS = HANDOVER_LABELS;
  HANDOVER_PLACEHOLDERS = HANDOVER_PLACEHOLDERS;
  HANDOVER_BUTTONS = HANDOVER_BUTTONS;
  HANDOVER_MESSAGES = HANDOVER_MESSAGES;

  readonly commitmentTypeOptions: SelectOption[] = [
    { value: 1, label: HANDOVER_LABELS.SOFTWARE },
    { value: 2, label: HANDOVER_LABELS.SERVICES },
    { value: 3, label: HANDOVER_LABELS.TRAINING },
  ];

  readonly timelineUnitOptions: SelectOption[] = HANDOVER_TIMELINE_UNITS.map((value) => ({
    value,
    label: value,
  }));

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    commitmentTypeId: [1, [Validators.required]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    timeline: [1, [Validators.required, Validators.min(1)]],
    timelineUnit: ['Weeks', [Validators.required]],
  });

  isSubmitting = signal(false);
  errorMessage = signal<string>('');
  isEditMode = signal(false);

  constructor() {
    if (this.data.commitment) {
      this.isEditMode.set(true);
      this.form.patchValue({
        title: this.data.commitment.title,
        commitmentTypeId: this.data.commitment.commitmentTypeId,
        description: this.data.commitment.description,
        timeline: this.data.commitment.timeline,
        timelineUnit: this.data.commitment.timelineUnit,
      });
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

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const request: CreateCommitmentRequest = this.form.getRawValue();
    const isEdit = this.isEditMode() && this.data.commitment;

    const request$ = isEdit
      ? this.handoverService.updateCommitment(this.data.engagementId, this.data.commitment!.commitmentId, request)
      : this.handoverService.addCommitment(this.data.engagementId, request);

    request$.subscribe({
      next: (response: CommitmentDto) => {
        this.isSubmitting.set(false);
        this.dialogRef.close(response);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const msg = err?.error?.message || (isEdit ? HANDOVER_MESSAGES.COMMITMENT_UPDATE_ERROR : HANDOVER_MESSAGES.COMMITMENT_ADD_ERROR);
        this.errorMessage.set(msg);
      },
    });
  }
}

