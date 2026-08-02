import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '../../../../shared/dialog/dialog-ref';
import { DIALOG_DATA } from '../../../../shared/dialog/dialog.service';
import { ChecklistCategory, ChecklistItem } from '../../../../models/checklist.model';

export interface ItemFormDialogData {
  mode: 'create' | 'edit';
  item: ChecklistItem | null;
  categories: ChecklistCategory[];
  nextDisplayOrder: number;
}

export interface ItemFormDialogResult {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
}

@Component({
  selector: 'app-item-form-dialog',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './item-form-dialog.html',
  styleUrl: './item-form-dialog.scss',
})
export class ItemFormDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<ItemFormDialogResult>>(DialogRef);
  readonly data = inject<ItemFormDialogData>(DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({
    title: [this.data.item?.title ?? '', [Validators.required]],
    description: [this.data.item?.description ?? ''],
    categoryId: [this.data.item?.categoryId ?? this.data.categories[0]?.id ?? null, [Validators.required]],
    isMandatory: [this.data.item?.isMandatory ?? true],
  });

  get isEdit(): boolean {
    return this.data.mode === 'edit';
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const result: ItemFormDialogResult = {
      title: value.title,
      description: value.description || null,
      categoryId: value.categoryId as number,
      isMandatory: value.isMandatory,
    };
    this.dialogRef.close(result);
  }
}
