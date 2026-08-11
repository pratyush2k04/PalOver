import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { CommunicationCadenceService } from '../../../../services/communication-cadence.service';
import { AuthService } from '../../../../../../services/auth';
import {
  CommunicationCadence,
  EngagementParticipant,
  CadenceTypeEnum
} from '../../../../models/cadence.model';
import { CADENCE_CONSTANTS } from '../../../../../../constants/cadence.constants';
import { AlertComponent } from '@common/components/alert/alert';
import { EmptyStateComponent } from '@common/components/empty-state/empty-state';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';

@Component({
  selector: 'app-governance-process',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AlertComponent,
    EmptyStateComponent,
    ButtonComponent,
    InputComponent,
    SelectComponent,
    MultiSelectListComponent
  ],
  templateUrl: './governance-process.component.html',
  styleUrl: './governance-process.component.scss'
})
export class GovernanceProcessComponent {
  private route = inject(ActivatedRoute);
  private cadenceService = inject(CommunicationCadenceService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  readonly constants = CADENCE_CONSTANTS;

  isProjectManager = computed(() => this.authService.hasRole('Project Management'));

  private paramMap = toSignal(this.route.paramMap);
  private parentParamMap = toSignal(this.route.parent?.paramMap ?? this.route.paramMap);

  engagementId = computed(() => {
    const idStr = this.paramMap()?.get('id') || this.parentParamMap()?.get('id');
    return idStr ? +idStr : null;
  });

  cadences = signal<CommunicationCadence[]>([]);
  assignedParticipants = signal<EngagementParticipant[]>([]);
  selectedParticipantUserIds = signal<number[]>([]);

  readonly cadenceOptions: SelectOption[] = [
    { value: CadenceTypeEnum.Daily, label: 'Daily' },
    { value: CadenceTypeEnum.Weekly, label: 'Weekly' },
    { value: CadenceTypeEnum.Fortnightly, label: 'Fortnightly' },
    { value: CadenceTypeEnum.Monthly, label: 'Monthly' },
    { value: CadenceTypeEnum.Quarterly, label: 'Quarterly' }
  ];

  readonly participantOptions = computed<SelectOption[]>(() =>
    this.assignedParticipants().map((participant) => ({
      value: participant.userId,
      label: participant.fullName
    }))
  );

  isModalOpen = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  cadenceForm: FormGroup = this.fb.group({
    stakeholderRole: ['', [Validators.required, Validators.maxLength(200)]],
    cadence: [CadenceTypeEnum.Weekly, [Validators.required]]
  });

  constructor() {
    effect(() => {
      const id = this.engagementId();
      if (id) {
        this.loadCadences(id);
      }
    });
  }

  loadCadences(engagementId: number): void {
    this.cadenceService.getCadences(engagementId).subscribe({
      next: (data) => this.cadences.set(data),
      error: (err) => console.error('Failed to load communication cadences:', err)
    });
  }

  openAddCadenceModal(): void {
    const id = this.engagementId();
    if (!id || !this.isProjectManager()) return;

    this.errorMessage.set(null);
    this.selectedParticipantUserIds.set([]);
    this.cadenceForm.reset({
      stakeholderRole: '',
      cadence: CadenceTypeEnum.Weekly
    });

    this.cadenceService.getAssignedParticipants(id).subscribe({
      next: (participants) => {
        this.assignedParticipants.set(participants);
        this.isModalOpen.set(true);
      },
      error: (err) => {
        console.error('Failed to load assigned participants:', err);
        this.assignedParticipants.set([]);
        this.isModalOpen.set(true);
      }
    });
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.errorMessage.set(null);
  }

  onParticipantSelectionChange(values: unknown[]): void {
    this.selectedParticipantUserIds.set(values as number[]);
  }

  onSubmitCadence(): void {
    if (this.cadenceForm.invalid) {
      this.cadenceForm.markAllAsTouched();
      return;
    }

    const participantIds = this.selectedParticipantUserIds();
    if (participantIds.length === 0) {
      this.errorMessage.set(this.constants.ERROR_PARTICIPANT_REQUIRED);
      return;
    }

    const id = this.engagementId();
    if (!id || !this.isProjectManager()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload = {
      stakeholderRole: this.cadenceForm.value.stakeholderRole,
      cadence: +this.cadenceForm.value.cadence,
      participantUserIds: participantIds
    };

    this.cadenceService.addCadence(id, payload).subscribe({
      next: (newCadence) => {
        this.isSubmitting.set(false);
        this.cadences.update(list => [...list, newCadence]);
        this.closeModal();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const backendMessage = err?.error?.message || err?.error || this.constants.ERROR_GENERIC;
        this.errorMessage.set(backendMessage);
      }
    });
  }
}
