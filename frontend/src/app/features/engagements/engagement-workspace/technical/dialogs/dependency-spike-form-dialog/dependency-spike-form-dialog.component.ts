import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { ToastService } from '@common/services/toast/toast';
import { AuthService } from '@shared/services/auth';
import { DependencySpikeService } from '../../../../services/dependency-spike.service';
import {
  DependencySpikeDto,
  DependencySpikeStatus,
  CreateDependencySpikeRequest,
  UpdateDependencySpikeRequest
} from '../../../../models/dependency-spikes/dependency-spike.model';

export interface DependencySpikeDialogData {
  engagementId: number;
  spike?: DependencySpikeDto | null;
}

@Component({
  selector: 'app-dependency-spike-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    InputComponent,
    TextareaComponent,
    SelectComponent
  ],
  templateUrl: './dependency-spike-form-dialog.component.html',
  styleUrl: './dependency-spike-form-dialog.component.scss'
})
export class DependencySpikeFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<DependencySpikeDto | null>>(DialogRef);
  readonly data = inject<DependencySpikeDialogData>(DIALOG_DATA);
  private spikeService = inject(DependencySpikeService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);

  isSubmitting = signal(false);
  form!: FormGroup;

  readonly statusOptions: SelectOption[] = [
    { value: DependencySpikeStatus.InProgress, label: 'In Progress' },
    { value: DependencySpikeStatus.Passed, label: 'Passed' },
    { value: DependencySpikeStatus.Failed, label: 'Failed' }
  ];

  get isEditMode(): boolean {
    return !!this.data?.spike?.id;
  }

  ngOnInit(): void {
    const spike = this.data?.spike;
    const initialStatus = spike?.status != null
      ? Number(spike.status)
      : DependencySpikeStatus.InProgress;

    this.form = this.fb.group({
      dependencyName: [
        spike?.dependencyName ?? '',
        [Validators.required, Validators.maxLength(200)]
      ],
      objective: [
        spike?.objective ?? '',
        [Validators.required, Validators.maxLength(1000)]
      ],
      status: [initialStatus, [Validators.required]],
      outcomeFindings: [
        spike?.outcomeFindings ?? '',
        [Validators.maxLength(2000)]
      ]
    });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formValue = this.form.value;
    const currentUserId = this.authService.getUserId() ?? undefined;

    const payload: CreateDependencySpikeRequest | UpdateDependencySpikeRequest = {
      dependencyName: formValue.dependencyName.trim(),
      objective: formValue.objective.trim(),
      status: Number(formValue.status),
      outcomeFindings: formValue.outcomeFindings?.trim() || null,
      ownerId: this.isEditMode ? undefined : currentUserId
    };

    if (this.isEditMode && this.data.spike?.id) {
      this.spikeService.updateSpike(this.data.engagementId, this.data.spike.id, payload).subscribe({
        next: (updatedSpike) => {
          this.toastService.success('Dependency spike updated successfully.');
          this.dialogRef.close(updatedSpike);
        },
        error: () => {
          this.isSubmitting.set(false);
        }
      });
    } else {
      this.spikeService.createSpike(this.data.engagementId, payload).subscribe({
        next: (createdSpike) => {
          this.toastService.success('Dependency spike created successfully.');
          this.dialogRef.close(createdSpike);
        },
        error: () => {
          this.isSubmitting.set(false);
        }
      });
    }
  }
}
