import { AbstractControl, ValidationErrors } from '@angular/forms';

export function hasUpperCase(value: string): boolean {
  return /[A-Z]/.test(value);
}

export function hasLowerCase(value: string): boolean {
  return /[a-z]/.test(value);
}

export function hasNumber(value: string): boolean {
  return /[0-9]/.test(value);
}

export function hasSymbol(value: string): boolean {
  return /[^A-Za-z0-9]/.test(value);
}

export function passwordStrengthValidator(control: AbstractControl | { value: string }): ValidationErrors | null {
  const value = control.value ?? '';
  const errors: ValidationErrors = {};

  if (value.length < 10) errors['minlength'] = true;
  if (!hasUpperCase(value)) errors['upper'] = true;
  if (!hasLowerCase(value)) errors['lower'] = true;
  if (!hasNumber(value)) errors['number'] = true;
  if (!hasSymbol(value)) errors['symbol'] = true;

  return Object.keys(errors).length ? errors : null;
}
