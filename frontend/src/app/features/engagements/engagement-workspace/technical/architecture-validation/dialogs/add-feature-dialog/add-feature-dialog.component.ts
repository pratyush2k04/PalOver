import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { CreateDiscoveryFeatureItemRequest } from '@features/engagements/models/architecture-validation/architecture-validation.requests';

@Component({
  selector: 'app-add-feature-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './add-feature-dialog.component.html',
  styleUrl: './add-feature-dialog.component.scss'
})
export class AddFeatureDialogComponent {
  private dialogRef = inject<DialogRef<CreateDiscoveryFeatureItemRequest>>(DialogRef);

  featureName = signal('');
  estimateWeeks = signal<number | null>(null);
  targetSprint = signal('');

  featureNameError = signal('');
  estimateWeeksError = signal('');
  targetSprintError = signal('');

  onFeatureNameChange(val: string): void {
    this.featureName.set(val);
    this.featureNameError.set('');
  }

  onEstimateWeeksChange(val: any): void {
    const num = val === '' || val === null ? null : Number(val);
    this.estimateWeeks.set(num);
    this.estimateWeeksError.set('');
  }

  onTargetSprintChange(val: string): void {
    this.targetSprint.set(val);
    this.targetSprintError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const nameVal = this.featureName().trim();
    const weeksVal = this.estimateWeeks();
    const sprintVal = this.targetSprint().trim();
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

    if (!sprintVal) {
      this.targetSprintError.set('Target sprint is required (e.g., Sprint 1)');
      isValid = false;
    } else if (sprintVal.length > 50) {
      this.targetSprintError.set('Target sprint cannot exceed 50 characters');
      isValid = false;
    }

    if (!isValid) {
      return;
    }

    this.dialogRef.close({
      featureName: nameVal,
      estimateWeeks: weeksVal!,
      targetSprint: sprintVal
    });
  }
}
