import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';

export interface AddEnvironmentItemDialogResult {
  name: string;
}

@Component({
  selector: 'app-add-environment-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './add-environment-item-dialog.component.html',
  styleUrl: './add-environment-item-dialog.component.scss'
})
export class AddEnvironmentItemDialogComponent {
  private dialogRef = inject<DialogRef<AddEnvironmentItemDialogResult>>(DialogRef);

  name = signal('');
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

    this.dialogRef.close({ name });
  }
}
