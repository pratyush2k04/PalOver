import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { CheckboxComponent } from '@common/components/form/checkbox/checkbox';

export interface AddAccessItemDialogResult {
  name: string;
  isClientSide: boolean;
  timeZoneNote?: string;
}

@Component({
  selector: 'app-add-access-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent, CheckboxComponent],
  templateUrl: './add-access-item-dialog.component.html',
  styleUrl: './add-access-item-dialog.component.scss'
})
export class AddAccessItemDialogComponent {
  private dialogRef = inject<DialogRef<AddAccessItemDialogResult>>(DialogRef);

  name = signal('');
  isClientSide = signal(true);
  timeZoneNote = signal('');
  nameError = signal('');

  onNameChange(value: string): void {
    this.name.set(value);
    this.nameError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const name = this.name().trim();
    if (!name) {
      this.nameError.set('This field is required');
      return;
    }

    this.dialogRef.close({
      name,
      isClientSide: this.isClientSide(),
      timeZoneNote: this.timeZoneNote().trim() || undefined
    });
  }
}
