import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import {
  CommercialModelOption,
  EngagementService,
  EngagementTypeOption
} from '../../services/engagement';

function billingNotBeforeKickoffValidator(group: AbstractControl): ValidationErrors | null {
  const kickoffDate = group.get('kickoffDate')?.value;
  const billingStartDate = group.get('billingStartDate')?.value;

  if (!kickoffDate || !billingStartDate) {
    return null;
  }

  return billingStartDate < kickoffDate ? { billingBeforeKickoff: true } : null;
}

@Component({
  selector: 'app-engagements',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './engagements.html',
  styleUrl: './engagements.scss'
})
export class EngagementsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private engagementService = inject(EngagementService);

  showModal = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);
  warnings = signal<string[]>([]);
  engagementTypes = signal<EngagementTypeOption[]>([]);
  commercialModels = signal<CommercialModelOption[]>([]);

  engagementForm: FormGroup = this.fb.group(
    {
      engagementName: ['', [Validators.required]],
      clientName: ['', [Validators.required]],
      engagementDescription: ['', [Validators.required]],
      engagementTypeId: [null, [Validators.required]],
      commercialModelId: [null, [Validators.required]],
      kickoffDate: ['', [Validators.required]],
      billingStartDate: ['', [Validators.required]]
    },
    { validators: billingNotBeforeKickoffValidator }
  );

  ngOnInit(): void {
    this.engagementService.getEngagementTypes().subscribe({
      next: (types) => {
        this.engagementTypes.set(types);
        if (types.length > 0) {
          this.engagementForm.patchValue({ engagementTypeId: types[0].id });
        }
      }
    });

    this.engagementService.getCommercialModels().subscribe({
      next: (models) => {
        this.commercialModels.set(models);
        if (models.length > 0) {
          this.engagementForm.patchValue({ commercialModelId: models[0].id });
        }
      }
    });
  }

  get engagementNameControl() {
    return this.engagementForm.get('engagementName');
  }

  get clientNameControl() {
    return this.engagementForm.get('clientName');
  }

  get engagementTypeIdControl() {
    return this.engagementForm.get('engagementTypeId');
  }

  get commercialModelIdControl() {
    return this.engagementForm.get('commercialModelId');
  }

  get engagementDescriptionControl() {
    return this.engagementForm.get('engagementDescription');
  }

  get kickoffDateControl() {
    return this.engagementForm.get('kickoffDate');
  }

  get billingStartDateControl() {
    return this.engagementForm.get('billingStartDate');
  }

  openModal(): void {
    this.errorMessage.set(null);
    this.warnings.set([]);
    this.engagementForm.reset({
      engagementTypeId: this.engagementTypes()[0]?.id ?? null,
      commercialModelId: this.commercialModels()[0]?.id ?? null
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSubmit(): void {
    if (this.engagementForm.invalid) {
      this.engagementForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    this.warnings.set([]);

    const { engagementName, clientName, engagementDescription, engagementTypeId, commercialModelId, kickoffDate, billingStartDate } =
      this.engagementForm.value;

    this.engagementService
      .markSowSigned({
        engagementName,
        clientName,
        engagementDescription: engagementDescription || undefined,
        engagementTypeId,
        commercialModelId,
        kickoffDate,
        billingStartDate: billingStartDate || undefined
      })
      .subscribe({
        next: (response) => {
          this.isSubmitting.set(false);
          this.warnings.set(response.warnings);
          this.showModal.set(false);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(
            typeof err.error === 'string' ? err.error : 'Failed to create engagement. Please try again.'
          );
        }
      });
  }
}
