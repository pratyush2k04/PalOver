import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface BlockEnvironmentItemDialogData {
  itemName: string;
  currentReason?: string;
}

export interface BlockEnvironmentItemDialogResult {
  reason: string;
}

@Component({
  selector: 'app-block-environment-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './block-environment-item-dialog.component.html',
  styleUrl: './block-environment-item-dialog.component.scss'
})
export class BlockEnvironmentItemDialogComponent {
  private dialogRef = inject<DialogRef<BlockEnvironmentItemDialogResult>>(DialogRef);
  readonly data = inject<BlockEnvironmentItemDialogData>(DIALOG_DATA);

  reason = signal(this.data.currentReason || '');
  reasonError = signal('');

  onReasonChange(value: string): void {
    this.reason.set(value);
    this.reasonError.set('');
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const reason = this.reason().trim();
    if (!reason) {
      this.reasonError.set('Reason is required to block an item');
      return;
    }
    if (reason.length > 500) {
      this.reasonError.set('Reason cannot exceed 500 characters');
      return;
    }

    this.dialogRef.close({ reason });
  }
}
