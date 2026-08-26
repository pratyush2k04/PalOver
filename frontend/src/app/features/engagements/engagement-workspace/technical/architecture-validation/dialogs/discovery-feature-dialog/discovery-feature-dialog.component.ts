import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { DiscoveryFeatureItemRequest } from '@features/engagements/models/architecture-validation/architecture-validation.requests';
import { DiscoveryFeatureItemDto } from '@features/engagements/models/architecture-validation/architecture-validation.dtos';

export interface DiscoveryFeatureDialogData {
  feature?: DiscoveryFeatureItemDto;
}

@Component({
  selector: 'app-discovery-feature-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './discovery-feature-dialog.component.html',
  styleUrl: './discovery-feature-dialog.component.scss'
})
export class DiscoveryFeatureDialogComponent {
  private dialogRef = inject<DialogRef<DiscoveryFeatureItemRequest>>(DialogRef);
  readonly data = inject<DiscoveryFeatureDialogData>(DIALOG_DATA, { optional: true });

  isEditMode = computed(() => !!this.data?.feature);
  dialogTitle = computed(() => this.isEditMode() ? 'Edit Discovery Feature' : 'Add Discovery Feature');
  submitButtonText = computed(() => this.isEditMode() ? 'Save Changes' : 'Add Feature');

  featureName = signal(this.data?.feature?.featureName ?? '');
  estimateWeeks = signal<number | null>(this.data?.feature?.estimateWeeks ?? null);
  
  targetSprint = signal<number | null>(null);

  featureNameError = signal('');
  estimateWeeksError = signal('');
  targetSprintError = signal('');

  constructor() {
    const sprintStr = this.data?.feature?.targetSprint;
    if (sprintStr) {
      const match = sprintStr.match(/\d+/);
      this.targetSprint.set(match ? Number(match[0]) : null);
    } else {
      this.targetSprint.set(null);
    }
  }

  onFeatureNameChange(val: string): void {
    this.featureName.set(val);
    this.featureNameError.set('');
  }

  onEstimateWeeksChange(val: any): void {
    const num = val === '' || val === null ? null : Number(val);
    this.estimateWeeks.set(num);
    this.estimateWeeksError.set('');
  }

  onTargetSprintChange(val: any): void {
    const num = val === '' || val === null ? null : Number(val);
    this.targetSprint.set(num);
    this.targetSprintError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const nameVal = this.featureName().trim();
    const weeksVal = this.estimateWeeks();
    const sprintVal = this.targetSprint();
    let isValid = true;

    if (!nameVal) {
      this.featureNameError.set('Feature name is required');
      isValid = false;
    } else if (nameVal.length > 200) {
      this.featureNameError.set('Feature name cannot exceed 200 characters');
      isValid = false;
    }

    if (weeksVal === null || isNaN(weeksVal) || weeksVal <= 0) {
      this.estimateWeeksError.set('Estimate must be greater than 0 weeks');
      isValid = false;
    } else if (weeksVal > 50) {
      this.estimateWeeksError.set('Estimate cannot exceed 50 weeks');
      isValid = false;
    }

    if (sprintVal === null || isNaN(sprintVal) || sprintVal <= 0) {
      this.targetSprintError.set('Target sprint is required (must be > 0)');
      isValid = false;
    }

    if (!isValid) {
      return;
    }

    this.dialogRef.close({
      featureName: nameVal,
      estimateWeeks: weeksVal!,
      targetSprint: `Sprint ${sprintVal}`
    });
  }
}
