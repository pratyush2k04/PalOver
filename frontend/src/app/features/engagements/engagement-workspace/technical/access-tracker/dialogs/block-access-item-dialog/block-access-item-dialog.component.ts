import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

export interface BlockAccessItemDialogData {
  itemName: string;
  currentReason?: string;
}

export interface BlockAccessItemDialogResult {
  reason: string;
}

@Component({
  selector: 'app-block-access-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './block-access-item-dialog.component.html',
  styleUrl: './block-access-item-dialog.component.scss'
})
export class BlockAccessItemDialogComponent {
  private dialogRef = inject<DialogRef<BlockAccessItemDialogResult>>(DialogRef);
  readonly data = inject<BlockAccessItemDialogData>(DIALOG_DATA);

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

    this.dialogRef.close({ reason });
  }
}
