import { Component, inject, signal } from '@angular/core';
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
import { ToastService } from '@common/services/toast/toast';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { MultiSelectListComponent } from '@common/components/multi-select-list/multi-select-list';
import {
  RESOURCE_REQUEST_CONSTANTS,
  STANDARD_ROLE_OPTIONS,
  SKILL_OPTIONS,
} from '@shared/constants/app.constants';
import {
  CreateResourceRequestPayload,
  ResourceRequestItem,
  StandardRoleEnum,
} from '../../../models/resource-request.model';
import { ResourceRequestService } from '../../../services/resource-request.service';

export interface RaiseResourceRequestDialogData {
  engagementId: number;
  billingStartDate?: string | null;
}

function notPastDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }
    const todayStr = new Date().toISOString().split('T')[0];
    return control.value < todayStr ? { pastDate: true } : null;
  };
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
    TextareaComponent,
    MultiSelectListComponent,
  ],
  templateUrl: './raise-resource-request-dialog.component.html',
  styleUrl: './raise-resource-request-dialog.component.scss',
})
export class RaiseResourceRequestDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<ResourceRequestItem | null>>(DialogRef);
  private resourceRequestService = inject(ResourceRequestService);
  private toastService = inject(ToastService);
  readonly data = inject<RaiseResourceRequestDialogData>(DIALOG_DATA);

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
        notPastDateValidator(),
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

  errorMessage = signal<string | null>(null);
  isSubmitting = signal<boolean>(false);

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

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const { standardRole, peopleNeeded, requiredByDate, jobDescription } = this.form.getRawValue();

    const payload: CreateResourceRequestPayload = {
      standardRole,
      skills: this.selectedSkills(),
      peopleNeeded,
      requiredByDate,
      jobDescription: jobDescription.trim(),
    };

    this.resourceRequestService
      .createResourceRequest(this.data.engagementId, payload)
      .subscribe({
        next: (created: ResourceRequestItem) => {
          this.toastService.success(RESOURCE_REQUEST_CONSTANTS.MSG_SUCCESS_RAISED);
          this.isSubmitting.set(false);
          this.dialogRef.close(created);
        },
        error: (err: { error?: { message?: string; title?: string } }) => {
          this.isSubmitting.set(false);
          const errorMsg =
            err.error?.message ||
            err.error?.title ||
            'Failed to raise resource request. Please try again.';
          this.errorMessage.set(errorMsg);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
