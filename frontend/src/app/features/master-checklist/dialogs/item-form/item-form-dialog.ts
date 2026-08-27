import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { MasterChecklistCategory, MasterChecklistItem } from '../../models/master-checklist.model';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { CheckboxComponent } from '@common/components/form/checkbox/checkbox';

export interface ItemFormDialogData {
  mode: 'create' | 'edit';
  item: MasterChecklistItem | null;
  categories: MasterChecklistCategory[];
  nextDisplayOrder: number;
}

export interface ItemFormDialogResult {
  title: string;
  description: string | null;
  categoryId: number;
  isMandatory: boolean;
  evidenceRequired: boolean;
}

@Component({
  selector: 'app-item-form-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonComponent, InputComponent, TextareaComponent, SelectComponent, CheckboxComponent],
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
    evidenceRequired: [this.data.item?.evidenceRequired ?? false],
  });

  get isEdit(): boolean {
    return this.data.mode === 'edit';
  }

  readonly categoryOptions: SelectOption[] = this.data.categories.map((category) => ({
    value: category.id,
    label: category.categoryName
  }));

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
      evidenceRequired: value.evidenceRequired,
    };
    this.dialogRef.close(result);
  }
}
