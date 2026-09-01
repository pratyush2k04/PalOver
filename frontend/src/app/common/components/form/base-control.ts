import { Directive, inject, input, model } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { resolveErrorMessage, shouldShowError } from './error-messages';

@Directive()
export abstract class BaseControlComponent<T> implements ControlValueAccessor {
  protected readonly ngControl = inject(NgControl, { optional: true, self: true });

  readonly errorMessages = input<Record<string, string>>({});
  readonly extraError = input<string | null>(null);
  readonly required = input<boolean>(false);

  abstract value: T;
  readonly disabled = model<boolean>(false);

  protected onChange: (value: T) => void = () => {};
  protected onTouched: () => void = () => {};

  constructor() {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  abstract writeValue(value: unknown): void;

  registerOnChange(fn: (value: T) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  markTouched(): void {
    this.onTouched();
  }

  get showError(): boolean {
    return shouldShowError(this.ngControl?.control, this.extraError());
  }

  get errorMessage(): string | null {
    return resolveErrorMessage(this.ngControl?.control, this.errorMessages(), this.extraError());
  }
}
