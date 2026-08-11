import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
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
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent, SelectComponent],
  templateUrl: './person-form-dialog.component.html',
  styleUrl: './person-form-dialog.component.scss'
})
export class PersonFormDialogComponent {
  private dialogRef = inject<DialogRef<PersonFormDialogResult>>(DialogRef);
  readonly data = inject<PersonFormDialogData>(DIALOG_DATA);

  readonly personTypeOptions: SelectOption[] = [
    { value: PersonType.Internal, label: 'Internal' },
    { value: PersonType.Client, label: 'Client' }
  ];

  private originalPersonType(): number {
    const person = this.data.person;
    if (!person) return PersonType.Internal;
    const numType = Number(person.personType);
    return numType === 2 || person.personType === 'Client' || person.personType === PersonType.Client
      ? PersonType.Client
      : PersonType.Internal;
  }

  formName = signal(this.data.person?.name ?? '');
  formRole = signal(this.data.person?.role ?? '');
  formPersonType = signal<number>(this.originalPersonType());

  isFormDirty = computed(() => {
    const person = this.data.person;
    if (!person) return true;
    return (
      this.formName().trim() !== person.name ||
      this.formRole().trim() !== person.role ||
      this.formPersonType() !== this.originalPersonType()
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
      return;
    }

    this.dialogRef.close({
      name,
      role,
      personType: Number(this.formPersonType())
    });
  }
}
