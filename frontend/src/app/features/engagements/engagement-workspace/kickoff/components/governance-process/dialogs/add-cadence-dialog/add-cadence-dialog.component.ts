import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import { CommunicationCadenceService } from '../../../../../../services/communication-cadence.service';
import { CommunicationCadence, EngagementParticipant, CadenceTypeEnum } from '../../../../../../models/cadence.model';
import { CADENCE_CONSTANTS } from '../../../../../../../../shared/constants/cadence.constants';

export interface AddCadenceDialogData {
  engagementId: number;
  participants: EngagementParticipant[];
}

@Component({
  selector: 'app-add-cadence-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AlertComponent,
    ButtonComponent,
    InputComponent,
    SelectComponent,
    MultiSelectListComponent
  ],
  templateUrl: './add-cadence-dialog.component.html',
  styleUrl: './add-cadence-dialog.component.scss'
})
export class AddCadenceDialogComponent {
  private dialogRef = inject<DialogRef<CommunicationCadence>>(DialogRef);
  private cadenceService = inject(CommunicationCadenceService);
  private fb = inject(FormBuilder);
  readonly data = inject<AddCadenceDialogData>(DIALOG_DATA);

  readonly constants = CADENCE_CONSTANTS;

  readonly cadenceOptions: SelectOption[] = [
    { value: CadenceTypeEnum.Daily, label: 'Daily' },
    { value: CadenceTypeEnum.Weekly, label: 'Weekly' },
    { value: CadenceTypeEnum.Fortnightly, label: 'Fortnightly' },
    { value: CadenceTypeEnum.Monthly, label: 'Monthly' },
    { value: CadenceTypeEnum.Quarterly, label: 'Quarterly' }
  ];

  readonly participantOptions: SelectOption[] = this.data.participants.map((participant) => ({
    value: participant.userId,
    label: participant.fullName
  }));

  selectedParticipantUserIds = signal<number[]>([]);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  cadenceForm: FormGroup = this.fb.group({
    stakeholderRole: ['', [Validators.required, Validators.maxLength(200)]],
    cadence: [CadenceTypeEnum.Weekly, [Validators.required]]
  });

  onParticipantSelectionChange(values: unknown[]): void {
    this.selectedParticipantUserIds.set(values as number[]);
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.cadenceForm.invalid) {
      this.cadenceForm.markAllAsTouched();
      return;
    }

    const participantIds = this.selectedParticipantUserIds();
    if (participantIds.length === 0) {
      this.errorMessage.set(this.constants.ERROR_PARTICIPANT_REQUIRED);
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      stakeholderRole: this.cadenceForm.value.stakeholderRole,
      cadence: +this.cadenceForm.value.cadence,
      participantUserIds: participantIds
    };

    this.cadenceService.addCadence(this.data.engagementId, payload).subscribe({
      next: (newCadence) => {
        this.isSubmitting.set(false);
        this.dialogRef.close(newCadence);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
    });
  }
}
