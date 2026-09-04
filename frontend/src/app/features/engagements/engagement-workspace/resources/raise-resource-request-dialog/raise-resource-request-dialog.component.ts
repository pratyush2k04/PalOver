import { Component, DestroyRef, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { InputComponent } from '@common/components/form/input/input';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import {
  RESOURCE_REQUEST_CONSTANTS,
  STANDARD_ROLE_OPTIONS,
  SKILL_OPTIONS,
} from '@shared/constants/app.constants';
import {
  CreateResourceRequestPayload,
  StandardRoleEnum,
} from '../../../models/resource-request.model';
import { futureDateValidator } from '@common/validators/date.validators';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { ResourceRequestService } from '@features/engagements/services/resource-request.service';

export interface RaiseResourceRequestDialogData {
  engagementId: number;
  billingStartDate?: string | null;
}

function notAfterBillingDateValidator(billingStartDate: string | null | undefined): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || !billingStartDate) {
      return null;
    }
    return control.value > billingStartDate ? { afterBillingDate: true } : null;
  };
}

@Component({
  selector: 'app-raise-resource-request-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    AlertComponent,
    SelectComponent,
    InputComponent,
    DatepickerComponent,
    TextareaComponent,
    MultiSelectListComponent,
  ],
  templateUrl: './raise-resource-request-dialog.component.html',
  styleUrl: './raise-resource-request-dialog.component.scss',
})
export class RaiseResourceRequestDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<CreateResourceRequestPayload>>(DialogRef);
  private engagementService = inject(EngagementService);
  private resourceRequestService = inject(ResourceRequestService);
  private destroyRef = inject(DestroyRef);
  readonly data = inject<RaiseResourceRequestDialogData>(DIALOG_DATA);

  readonly billingStartDate = computed(() => this.data.billingStartDate || this.engagementService.currentEngagement()?.billingStartDate || null);

  readonly minDate = new Date().toISOString().split('T')[0];
  readonly constants = RESOURCE_REQUEST_CONSTANTS;
  readonly roleOptions: SelectOption[] = [...STANDARD_ROLE_OPTIONS];
  readonly skillOptions: SelectOption[] = [...SKILL_OPTIONS];

  readonly form = this.fb.nonNullable.group({
    standardRole: [StandardRoleEnum.SeniorDeveloper as number, [Validators.required]],
    peopleNeeded: [1, [Validators.required, Validators.min(1)]],
    requiredByDate: [
      '',
      [
        Validators.required,
        futureDateValidator,
        notAfterBillingDateValidator(this.data.billingStartDate),
      ],
    ],
    jobDescription: [
      '',
      [
        Validators.required,
        Validators.minLength(RESOURCE_REQUEST_CONSTANTS.MIN_JOB_DESCRIPTION_LENGTH),
        Validators.maxLength(RESOURCE_REQUEST_CONSTANTS.MAX_JOB_DESCRIPTION_LENGTH),
      ],
    ],
  });

  // MultiSelectListComponent isn't a ControlValueAccessor, so skills live outside the
  // FormGroup as a signal — same pattern as custom-visibility-dialog's role selection.
  selectedSkills = signal<number[]>([]);
  skillsTouched = signal(false);

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  onSkillsChange(values: unknown[]): void {
    this.selectedSkills.set(values as number[]);
    this.skillsTouched.set(true);
  }

  onSubmit(): void {
    const skillsValid = this.selectedSkills().length > 0;

    if (this.form.invalid || !skillsValid) {
      this.form.markAllAsTouched();
      this.skillsTouched.set(true);
      return;
    }

    const { standardRole, peopleNeeded, requiredByDate, jobDescription } = this.form.getRawValue();

    const payload: CreateResourceRequestPayload = {
      standardRole,
      skills: this.selectedSkills(),
      peopleNeeded,
      requiredByDate,
      jobDescription: jobDescription.trim(),
    };

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.resourceRequestService
      .createResourceRequest(this.data.engagementId, payload, true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.dialogRef.close(payload);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(err?.error?.message || err?.error || 'Failed to raise resource request. Please try again.');
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
