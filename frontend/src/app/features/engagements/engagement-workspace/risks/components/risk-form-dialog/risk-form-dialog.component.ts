import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { Risk, RiskImpact, RiskLikelihood, RiskStatus, OwnerOption } from '../../models/risk.model';

export interface RiskFormDialogData {
  engagementId: number;
  risk: Risk | null;
  owners: OwnerOption[];
}

export interface RiskFormDialogResult {
  title: string;
  description: string;
  impact: RiskImpact;
  likelihood: RiskLikelihood;
  ownerId: number | null;
  mitigation: string | null;
  status?: RiskStatus;
}

@Component({
  selector: 'app-risk-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, SelectComponent, TextareaComponent],
  templateUrl: './risk-form-dialog.component.html',
  styleUrl: './risk-form-dialog.component.scss'
})
export class RiskFormDialogComponent {
  private dialogRef = inject<DialogRef<RiskFormDialogResult>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<RiskFormDialogData>(DIALOG_DATA);

  readonly isEditMode = !!this.data.risk;

  readonly impactOptions: SelectOption[] = [
    { value: 'Low', label: 'Low' },
    { value: 'Medium', label: 'Medium' },
    { value: 'High', label: 'High' }
  ];

  readonly likelihoodOptions: SelectOption[] = [
    { value: 'Low', label: 'Low' },
    { value: 'Medium', label: 'Medium' },
    { value: 'High', label: 'High' }
  ];

  readonly statusEditOptions: SelectOption[] = [
    { value: 'Open', label: 'Open' },
    { value: 'Mitigating', label: 'Mitigating' }
  ];

  readonly ownerSelectOptions: SelectOption[] = [
    { value: null, label: '-- Select Owner --' },
    ...this.data.owners.map((o) => ({
      value: o.id,
      label: o.role ? `${o.name} (${o.role})` : o.name
    }))
  ];

  riskForm: FormGroup = this.fb.group({
    title: [this.data.risk?.title ?? '', [Validators.required, Validators.maxLength(300)]],
    description: [this.data.risk?.description ?? '', [Validators.required, Validators.maxLength(2000)]],
    impact: [this.data.risk?.impact ?? 'Medium', [Validators.required]],
    likelihood: [this.data.risk?.likelihood ?? 'Medium', [Validators.required]],
    ownerId: [this.data.risk?.ownerId != null ? Number(this.data.risk.ownerId) : null, [Validators.required]],
    mitigation: [this.data.risk?.mitigation ?? '', [Validators.maxLength(2000)]],
    status: [this.data.risk?.status ?? 'Open', [Validators.required]]
  });

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.riskForm.invalid) {
      this.riskForm.markAllAsTouched();
      return;
    }

    const val = this.riskForm.value;
    const result: RiskFormDialogResult = {
      title: val.title,
      description: val.description,
      impact: val.impact as RiskImpact,
      likelihood: val.likelihood as RiskLikelihood,
      ownerId: val.ownerId ? +val.ownerId : null,
      mitigation: val.mitigation
    };

    if (this.isEditMode) {
      result.status = val.status as RiskStatus;
    }

    this.dialogRef.close(result);
  }
}
