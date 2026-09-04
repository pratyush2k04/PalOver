import { Component, OnInit, computed, inject, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { EngagementService } from '../../services/engagement.service';
import { CommercialModelOption, EngagementTypeOption } from '../../models/engagement.model';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { AlertComponent } from '@common/components/alert/alert';
import { ToastService } from '@common/services/toast/toast';
import { futureDateValidator } from '@common/validators/date.validators';

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
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent, TextareaComponent, SelectComponent, DatepickerComponent, AlertComponent],
  templateUrl: './engagements.html',
  styleUrl: './engagements.scss'
})
export class EngagementsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private engagementService = inject(EngagementService);
  private toastService = inject(ToastService);

  readonly minDate = new Date().toISOString().split('T')[0];
  showModal = signal(false);
  engagementTypes = signal<EngagementTypeOption[]>([]);
  commercialModels = signal<CommercialModelOption[]>([]);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  engagementCreated = output<void>();

  engagementForm: FormGroup = this.fb.group(
    {
      engagementName: ['', [Validators.required]],
      clientName: ['', [Validators.required]],
      engagementDescription: ['', [Validators.required]],
      engagementTypeId: [null, [Validators.required]],
      commercialModelId: [null, [Validators.required]],
      kickoffDate: ['', [Validators.required, futureDateValidator]],
      billingStartDate: ['', [Validators.required, futureDateValidator]]
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

  readonly engagementTypeOptions = computed<SelectOption[]>(() =>
    this.engagementTypes().map((type) => ({ value: type.id, label: type.name }))
  );

  readonly commercialModelOptions = computed<SelectOption[]>(() =>
    this.commercialModels().map((model) => ({ value: model.id, label: model.name }))
  );

  openModal(): void {
    this.engagementForm.reset({
      engagementTypeId: this.engagementTypes()[0]?.id ?? null,
      commercialModelId: this.commercialModels()[0]?.id ?? null
    });
    this.errorMessage.set(null);
    this.isSubmitting.set(false);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.errorMessage.set(null);
    this.isSubmitting.set(false);
  }

  onSubmit(): void {
    if (this.engagementForm.invalid) {
      this.engagementForm.markAllAsTouched();
      return;
    }

    const { engagementName, clientName, engagementDescription, engagementTypeId, commercialModelId, kickoffDate, billingStartDate } =
      this.engagementForm.value;

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.engagementService
      .markSowSigned(
        {
          engagementName,
          clientName,
          engagementDescription: engagementDescription || undefined,
          engagementTypeId,
          commercialModelId,
          kickoffDate,
          billingStartDate: billingStartDate || undefined
        },
        true
      )
      .subscribe({
        next: (response) => {
          this.closeModal();
          if (response.warnings && response.warnings.length > 0) {
            response.warnings.forEach(warning => this.toastService.error(warning));
          }
          this.engagementCreated.emit();
        },
        error: (err) => {
          this.isSubmitting.set(false);
          this.errorMessage.set(
            err?.error?.message || (typeof err?.error === 'string' ? err.error : 'Failed to create engagement. Please try again.')
          );
        }
      });
  }
}
