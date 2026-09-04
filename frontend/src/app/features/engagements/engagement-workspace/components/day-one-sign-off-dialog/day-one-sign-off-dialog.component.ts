import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { DatepickerComponent } from '@common/components/form/datepicker/datepicker';
import { DayOneSignOffRequest } from '@features/engagements/models/engagement.model';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { EngagementService } from '@features/engagements/services/engagement.service';
import { ToastService } from '@common/services/toast/toast';
import { futureDateValidator } from '@common/validators/date.validators';

@Component({
  selector: 'app-day-one-sign-off-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, AlertComponent, DatepickerComponent],
  templateUrl: './day-one-sign-off-dialog.component.html',
  styleUrl: './day-one-sign-off-dialog.component.scss'
})
export class DayOneSignOffDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(DialogRef<boolean>);
  public data = inject(DIALOG_DATA) as {
    engagementId: number;
    isAbleToStartDayOne: boolean;
    isLeadership: boolean;
    initialBillingStartDate: string;
  };
  private engagementService = inject(EngagementService);
  private toastService = inject(ToastService);

  readonly minDate = new Date().toISOString().split('T')[0];
  readonly isBusy = signal<boolean>(false);
  readonly isException = computed(() => !this.data.isAbleToStartDayOne);

  readonly form: FormGroup = this.fb.group({
    billingStartDate: [this.data.initialBillingStartDate || '', [Validators.required, futureDateValidator]],
    exceptionJustification: ['']
  });

  private readonly formStatus = toSignal(this.form.statusChanges, { initialValue: this.form.status });
  private readonly formValue = toSignal(this.form.valueChanges, { initialValue: this.form.value });

  readonly justificationLength = computed(() => {
    return this.formValue()?.exceptionJustification?.length ?? 0;
  });

  readonly canSubmit = computed(() => {
    if (this.isBusy()) return false;
    if (this.isException() && !this.data.isLeadership) return false;
    return this.formStatus() === 'VALID';
  });

  constructor() {
    effect(() => {
      const isBlockedAndNotLeadership = !this.data.isAbleToStartDayOne && !this.data.isLeadership;
      const busy = this.isBusy();

      if (busy || isBlockedAndNotLeadership) {
        this.form.get('billingStartDate')?.disable({ emitEvent: false });
      } else {
        this.form.get('billingStartDate')?.enable({ emitEvent: false });
      }

      if (busy) {
        this.form.get('exceptionJustification')?.disable({ emitEvent: false });
      } else {
        this.form.get('exceptionJustification')?.enable({ emitEvent: false });
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
    if (!this.isBusy()) {
      this.dialogRef.close(false);
    }
  }

  onSubmit(): void {
    if (!this.canSubmit()) return;

    const val = this.form.getRawValue();
    const request: DayOneSignOffRequest = {
      billingStartDate: val.billingStartDate,
      isException: this.isException(),
      exceptionJustification: this.isException() ? val.exceptionJustification?.trim() : undefined
    };

    this.isBusy.set(true);
    this.engagementService.signOffDayOne(this.data.engagementId, request).subscribe({
      next: () => {
        this.isBusy.set(false);
        this.toastService.success('Day-1 successfully signed off!');
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isBusy.set(false);
        this.toastService.error(err.error?.message || 'Failed to sign off Day-1 readiness.');
      }
    });
  }
}
