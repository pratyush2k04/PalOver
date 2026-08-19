import { Component, computed, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { PersonDto, PersonType } from 'src/app/models/person.model';

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
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './person-form-dialog.component.html',
  styleUrl: './person-form-dialog.component.scss'
})
export class PersonFormDialogComponent {
  @ViewChild(NgForm) form!: NgForm;

  private dialogRef = inject<DialogRef<PersonFormDialogResult>>(DialogRef);
  readonly data = inject<PersonFormDialogData>(DIALOG_DATA);

  formName = signal(this.data.person?.name ?? '');
  formRole = signal(this.data.person?.role ?? '');

  isFormDirty = computed(() => {
    const person = this.data.person;
    if (!person) return true;
    return (
      this.formName().trim() !== person.name ||
      this.formRole().trim() !== person.role
    );
  });

  get isEditing(): boolean {
    return !!this.data.person;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const name = this.formName().trim();
    const role = this.formRole().trim();

    if (!name || !role) {
      this.form.control.markAllAsTouched();
      return;
    }

    if (this.isEditing && !this.isFormDirty()) {
      this.dialogRef.close();
      return;
    }

    this.dialogRef.close({
      name,
      role,
      personType: PersonType.Client
    });
  }
}
