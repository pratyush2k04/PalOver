import { Component, computed, effect, inject, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { ButtonComponent } from '@common/components/button/button';
import { AlertComponent } from '@common/components/alert/alert';
import { DayOneSignOffRequest } from '@features/engagements/models/engagement.model';

@Component({
  selector: 'app-day-one-sign-off-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, AlertComponent],
  templateUrl: './day-one-sign-off-dialog.component.html',
  styleUrl: './day-one-sign-off-dialog.component.scss'
})
export class DayOneSignOffDialogComponent {
  private fb = inject(FormBuilder);

  // Signal inputs
  readonly isAbleToStartDayOne = input<boolean>(true);
  readonly isLeadership = input<boolean>(false);
  readonly initialBillingStartDate = input<string>('');
  readonly isBusy = input<boolean>(false);

  // Signal outputs
  readonly confirmed = output<DayOneSignOffRequest>();
  readonly cancelled = output<void>();

  // Computed derived state
  readonly isException = computed(() => !this.isAbleToStartDayOne());

  readonly form: FormGroup = this.fb.group({
    billingStartDate: ['', Validators.required],
    exceptionJustification: ['']
  });

  private readonly formStatus = toSignal(this.form.statusChanges, { initialValue: this.form.status });
  private readonly formValue = toSignal(this.form.valueChanges, { initialValue: this.form.value });

  readonly justificationLength = computed(() => {
    return this.formValue()?.exceptionJustification?.length ?? 0;
  });

  readonly canSubmit = computed(() => {
    if (this.isBusy()) return false;
    if (this.isException() && !this.isLeadership()) return false;
    return this.formStatus() === 'VALID';
  });

  constructor() {
    effect(() => {
      const initialDate = this.initialBillingStartDate();
      if (initialDate) {
        this.form.patchValue({ billingStartDate: initialDate }, { emitEvent: true });
      }
    });

    effect(() => {
      const isBlockedAndNotLeadership = !this.isAbleToStartDayOne() && !this.isLeadership();
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
          justificationCtrl.setValidators([Validators.required, Validators.minLength(20)]);
        } else {
          justificationCtrl.clearValidators();
        }
        justificationCtrl.updateValueAndValidity({ emitEvent: true });
      }
    });
  }

  onBackdropClick(): void {
    if (!this.isBusy()) {
      this.cancelled.emit();
    }
  }

  onDialogClick(event: MouseEvent): void {
    event.stopPropagation();
  }

  onSubmit(): void {
    if (!this.canSubmit()) return;

    const val = this.form.getRawValue();
    const request: DayOneSignOffRequest = {
      billingStartDate: val.billingStartDate,
      isException: this.isException(),
      exceptionJustification: this.isException() ? val.exceptionJustification?.trim() : undefined
    };

    this.confirmed.emit(request);
  }
}
