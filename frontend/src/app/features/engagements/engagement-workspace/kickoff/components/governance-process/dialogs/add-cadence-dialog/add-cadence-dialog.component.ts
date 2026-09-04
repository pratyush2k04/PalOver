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
import { CreateCommunicationCadence, EngagementParticipant, CadenceTypeEnum } from '../../../../../../models/cadence.model';
import { CADENCE_CONSTANTS } from '@shared/constants/cadence.constants';

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
  private dialogRef = inject<DialogRef<CreateCommunicationCadence>>(DialogRef);
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

    this.errorMessage.set(null);
    this.dialogRef.close({
      stakeholderRole: this.cadenceForm.value.stakeholderRole,
      cadence: +this.cadenceForm.value.cadence,
      participantUserIds: participantIds
    });
  }
}
