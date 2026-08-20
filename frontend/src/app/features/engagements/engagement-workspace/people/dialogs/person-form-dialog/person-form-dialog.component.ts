import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { PersonDto, PersonType } from '@shared/models/person.model';

export interface PersonFormDialogData {
  person: PersonDto | null;
}

export interface PersonFormDialogResult {
  name: string;
  role: string;
  personType: number;
}

@Component({
  selector: 'app-person-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './person-form-dialog.component.html',
  styleUrl: './person-form-dialog.component.scss'
})
export class PersonFormDialogComponent {
  private dialogRef = inject<DialogRef<PersonFormDialogResult>>(DialogRef);
  private fb = inject(FormBuilder);
  readonly data = inject<PersonFormDialogData>(DIALOG_DATA);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    role: ['', [Validators.required, Validators.maxLength(200)]],
  });

  get isEditing(): boolean {
    return !!this.data.person;
  }

  constructor() {
    if (this.data.person) {
      this.form.patchValue({
        name: this.data.person.name,
        role: this.data.person.role,
      });
      this.form.markAsPristine();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.isEditing && !this.form.dirty) {
      this.dialogRef.close();
      return;
    }

    this.dialogRef.close({
      name: this.form.value.name!.trim(),
      role: this.form.value.role!.trim(),
      personType: PersonType.Client
    });
  }
}
