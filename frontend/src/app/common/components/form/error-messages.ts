import { AbstractControl } from '@angular/forms';

export const DEFAULT_ERROR_MESSAGES: Record<string, string> = {
  required: 'This field is required.',
  email: 'Enter a valid email address.',
  maxlength: 'Maximum length exceeded.'
};

export function shouldShowError(control: AbstractControl | null | undefined, extraError: string | null): boolean {
  if (extraError) {
    return true;
  }
  return !!control && control.invalid && (control.touched || control.dirty);
}

export function resolveErrorMessage(
  control: AbstractControl | null | undefined,
  errorMessages: Record<string, string>,
  extraError: string | null
): string | null {
  if (extraError) {
    return extraError;
  }
  if (!control || !control.errors) {
    return null;
  }
  const key = Object.keys(control.errors)[0];
  return errorMessages[key] ?? DEFAULT_ERROR_MESSAGES[key] ?? 'This field is invalid.';
}
