import { AbstractControl, ValidationErrors } from '@angular/forms';

/**
 * Validates that the selected date is today or in the future.
 * Returns `{ pastDate: true }` if the date is in the past.
 */
export function futureDateValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) {
    return null;
  }

  let inputDate: Date;
  if (typeof value === 'string') {
    const datePart = value.split('T')[0];
    const parts = datePart.split('-');
    if (parts.length === 3) {
      const year = parseInt(parts[0], 10);
      const month = parseInt(parts[1], 10) - 1;
      const day = parseInt(parts[2], 10);
      inputDate = new Date(year, month, day);
    } else {
      inputDate = new Date(value);
    }
  } else if (value instanceof Date) {
    inputDate = new Date(value.getFullYear(), value.getMonth(), value.getDate());
  } else {
    return null;
  }

  if (isNaN(inputDate.getTime())) {
    return null;
  }

  inputDate.setHours(0, 0, 0, 0);

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (inputDate.getTime() < today.getTime()) {
    return { pastDate: true };
  }

  return null;
}
