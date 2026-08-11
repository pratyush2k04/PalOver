import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from 'src/app/shared/dialog/dialog-ref';
import { DIALOG_DATA } from 'src/app/shared/dialog/dialog.service';
import { PersonDto } from 'src/app/models/person.model';

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
  imports: [CommonModule, ReactiveFormsModule],
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
