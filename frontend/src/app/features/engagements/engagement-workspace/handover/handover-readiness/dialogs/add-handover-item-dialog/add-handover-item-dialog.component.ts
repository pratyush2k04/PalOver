import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { AddHandoverItemDialogResult } from '@features/engagements/models/handover-readiness.model';

@Component({
  selector: 'app-add-handover-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  templateUrl: './add-handover-item-dialog.component.html',
  styleUrl: './add-handover-item-dialog.component.scss'
})
export class AddHandoverItemDialogComponent {
  private dialogRef = inject<DialogRef<AddHandoverItemDialogResult>>(DialogRef);

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
      this.nameError.set('Item name is required');
      return;
    }

    this.dialogRef.close({ name });
  }
}
