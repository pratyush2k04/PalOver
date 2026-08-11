import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { PersonDto } from 'src/app/models/person.model';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';

export interface BlockItemDialogData {
  itemTitle: string;
  currentOwnerId: number | null;
  people: PersonDto[];
}

export interface BlockItemDialogResult {
  reason: string;
  personId: number;
}

@Component({
  selector: 'app-block-item-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent, SelectComponent],
  templateUrl: './block-item-dialog.component.html',
  styleUrl: './block-item-dialog.component.scss'
})
export class BlockItemDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<BlockItemDialogResult>>(DialogRef);
  readonly data = inject<BlockItemDialogData>(DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({
    reason: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    personId: [this.data.currentOwnerId ?? null as number | null, [Validators.required]]
  });

  get reasonControl() {
    return this.form.get('reason')!;
  }

  get personIdControl() {
    return this.form.get('personId')!;
  }

  get reasonLength(): number {
    return this.reasonControl.value?.length || 0;
  }

  readonly ownerOptions: SelectOption[] = this.data.people.map((person) => ({
    value: person.personId,
    label: person.name
  }));

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const reason = this.reasonControl.value.trim();
    const personId = Number(this.personIdControl.value);
    this.dialogRef.close({ reason, personId });
  }
}
