import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { ToastService } from '@common/services/toast/toast';
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

@Component({
  selector: 'app-raise-resource-request-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InlineSVGModule, ButtonComponent, AlertComponent],
  templateUrl: './raise-resource-request-dialog.component.html',
  styleUrl: './raise-resource-request-dialog.component.scss',
})
export class RaiseResourceRequestDialogComponent {
  private dialogRef = inject<DialogRef<ResourceRequestItem | null>>(DialogRef);
  private resourceRequestService = inject(ResourceRequestService);
  private toastService = inject(ToastService);
  readonly data = inject<RaiseResourceRequestDialogData>(DIALOG_DATA);

  readonly constants = RESOURCE_REQUEST_CONSTANTS;
  readonly roleOptions = STANDARD_ROLE_OPTIONS;
  readonly skillOptions = SKILL_OPTIONS;

  selectedRole = signal<number>(StandardRoleEnum.SeniorDeveloper);
  selectedSkills = signal<number[]>([]);
  peopleNeeded = signal<number>(1);
  requiredByDate = signal<string>('');
  jobDescription = signal<string>('');

  errorMessage = signal<string | null>(null);
  isSubmitting = signal<boolean>(false);

  readonly descLength = computed(() => this.jobDescription().trim().length);

  readonly isFormValid = computed(() => {
    const roleValid = this.selectedRole() > 0;
    const skillsValid = this.selectedSkills().length > 0;
    const peopleValid = this.peopleNeeded() > 0;
    const dateValid = !!this.requiredByDate();
    const descLength = this.descLength();
    const descValid =
      descLength >= RESOURCE_REQUEST_CONSTANTS.MIN_JOB_DESCRIPTION_LENGTH &&
      descLength <= RESOURCE_REQUEST_CONSTANTS.MAX_JOB_DESCRIPTION_LENGTH;

    return roleValid && skillsValid && peopleValid && dateValid && descValid;
  });

  onRoleChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.selectedRole.set(Number(select.value));
    this.clearError();
  }

  toggleSkill(skillValue: number): void {
    const current = this.selectedSkills();
    if (current.includes(skillValue)) {
      this.selectedSkills.set(current.filter((s) => s !== skillValue));
    } else {
      this.selectedSkills.set([...current, skillValue]);
    }
    this.clearError();
  }

  isSkillSelected(skillValue: number): boolean {
    return this.selectedSkills().includes(skillValue);
  }

  onPeopleChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const val = Number(input.value);
    this.peopleNeeded.set(val > 0 ? val : 1);
    this.clearError();
  }

  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.requiredByDate.set(input.value);
    this.clearError();
  }

  onJobDescriptionInput(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    this.jobDescription.set(textarea.value);
    this.clearError();
  }

  private clearError(): void {
    if (this.errorMessage()) {
      this.errorMessage.set(null);
    }
  }

  validate(): boolean {
    if (this.selectedSkills().length === 0) {
      this.errorMessage.set(RESOURCE_REQUEST_CONSTANTS.MSG_SKILL_REQUIRED);
      return false;
    }

    if (this.peopleNeeded() <= 0) {
      this.errorMessage.set(RESOURCE_REQUEST_CONSTANTS.MSG_PEOPLE_NEEDED_POSITIVE);
      return false;
    }

    if (!this.requiredByDate()) {
      this.errorMessage.set('Please select a required-by date.');
      return false;
    }

    const todayStr = new Date().toISOString().split('T')[0];
    if (this.requiredByDate() < todayStr) {
      this.errorMessage.set(RESOURCE_REQUEST_CONSTANTS.MSG_REQUIRED_DATE_PAST);
      return false;
    }

    if (this.data.billingStartDate && this.requiredByDate() > this.data.billingStartDate) {
      this.errorMessage.set(RESOURCE_REQUEST_CONSTANTS.MSG_REQUIRED_BEFORE_BILLING);
      return false;
    }

    const descLength = this.descLength();
    if (
      descLength < RESOURCE_REQUEST_CONSTANTS.MIN_JOB_DESCRIPTION_LENGTH ||
      descLength > RESOURCE_REQUEST_CONSTANTS.MAX_JOB_DESCRIPTION_LENGTH
    ) {
      this.errorMessage.set(RESOURCE_REQUEST_CONSTANTS.MSG_JOB_DESC_LENGTH);
      return false;
    }

    return true;
  }

  onSubmit(): void {
    if (!this.validate()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const payload: CreateResourceRequestPayload = {
      standardRole: this.selectedRole(),
      skills: this.selectedSkills(),
      peopleNeeded: this.peopleNeeded(),
      requiredByDate: this.requiredByDate(),
      jobDescription: this.jobDescription().trim(),
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
