import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { DayOneSignOffRequest } from '@features/engagements/models/engagement.model';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { futureDateValidator } from '@common/validators/date.validators';

@Component({
  selector: 'app-day-one-sign-off-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, AlertComponent, DatepickerComponent, TextareaComponent],
  templateUrl: './day-one-sign-off-dialog.component.html',
  styleUrl: './day-one-sign-off-dialog.component.scss'
})
export class DayOneSignOffDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(DialogRef<boolean>);
  private engagementService = inject(EngagementService);
  public data = inject(DIALOG_DATA) as {
    engagementId: number;
    isAbleToStartDayOne: boolean;
    isLeadership: boolean;
    initialBillingStartDate: string;
  };

  readonly minDate = new Date().toISOString().split('T')[0];
  readonly isException = computed(() => !this.data.isAbleToStartDayOne);

  readonly form: FormGroup = this.fb.group({
    billingStartDate: [this.data.initialBillingStartDate || '', [Validators.required, futureDateValidator]],
    exceptionJustification: ['']
  });

  readonly canAttemptSubmit = computed(() => !(this.isException() && !this.data.isLeadership));

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    effect(() => {
      const isBlockedAndNotLeadership = !this.data.isAbleToStartDayOne && !this.data.isLeadership;

      if (isBlockedAndNotLeadership) {
        this.form.get('billingStartDate')?.disable({ emitEvent: false });
      } else {
        this.form.get('billingStartDate')?.enable({ emitEvent: false });
      }
    });

    effect(() => {
      const exception = this.isException();
      const justificationCtrl = this.form.get('exceptionJustification');
      if (justificationCtrl) {
        if (exception) {
          justificationCtrl.setValidators([
            Validators.required,
            Validators.minLength(20),
            Validators.maxLength(500)
          ]);
        } else {
          justificationCtrl.clearValidators();
        }
        justificationCtrl.updateValueAndValidity({ emitEvent: true });
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.canAttemptSubmit()) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const val = this.form.getRawValue();
    const request: DayOneSignOffRequest = {
      billingStartDate: val.billingStartDate,
      isException: this.isException(),
      exceptionJustification: this.isException() ? val.exceptionJustification?.trim() : undefined
    };

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.engagementService.signOffDayOne(this.data.engagementId, request, true).subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || err?.error || 'Something went wrong');
      }
    });
  }
}
