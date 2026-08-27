import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';

import { BlockHandoverItemDialogData, BlockHandoverItemDialogResult } from '@features/engagements/models/handover-readiness.model';

@Component({
  selector: 'app-block-handover-item-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, TextareaComponent],
  templateUrl: './block-handover-item-dialog.component.html',
  styleUrl: './block-handover-item-dialog.component.scss'
})
export class BlockHandoverItemDialogComponent {
  private dialogRef = inject<DialogRef<BlockHandoverItemDialogResult>>(DialogRef);
  readonly data = inject<BlockHandoverItemDialogData>(DIALOG_DATA);

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
      this.reasonError.set('Reason is required to block a readiness item');
      return;
    }

    this.dialogRef.close({ reason });
  }
}
