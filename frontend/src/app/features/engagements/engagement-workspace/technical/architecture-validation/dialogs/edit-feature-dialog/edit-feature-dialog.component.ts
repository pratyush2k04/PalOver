import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { UpdateDiscoveryFeatureItemRequest } from '@features/engagements/models/architecture-validation/architecture-validation.requests';
import { DiscoveryFeatureItemDto } from '@features/engagements/models/architecture-validation/architecture-validation.dtos';

export interface EditFeatureDialogData {
  feature: DiscoveryFeatureItemDto;
}

@Component({
  selector: 'app-edit-feature-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './edit-feature-dialog.component.html',
  styleUrl: './edit-feature-dialog.component.scss'
})
export class EditFeatureDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<UpdateDiscoveryFeatureItemRequest>>(DialogRef);
  readonly data = inject<EditFeatureDialogData>(DIALOG_DATA, { optional: true });

  featureName = signal('');
  estimateWeeks = signal<number | null>(null);
  targetSprint = signal('');

  featureNameError = signal('');
  estimateWeeksError = signal('');
  targetSprintError = signal('');

  ngOnInit(): void {
    if (this.data?.feature) {
      this.featureName.set(this.data.feature.featureName);
      this.estimateWeeks.set(this.data.feature.estimateWeeks);
      this.targetSprint.set(this.data.feature.targetSprint);
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
      this.targetSprintError.set('Target sprint is required');
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
