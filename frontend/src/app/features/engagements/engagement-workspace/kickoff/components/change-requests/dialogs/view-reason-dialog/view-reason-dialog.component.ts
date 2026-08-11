import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { CHANGE_REQUEST_CONSTANTS } from 'src/app/constants/change-request.constants';

export interface ViewReasonDialogData {
  reason: string;
}

@Component({
  selector: 'app-view-reason-dialog',
  standalone: true,
  imports: [CommonModule, ButtonComponent],
  templateUrl: './view-reason-dialog.component.html',
  styleUrl: './view-reason-dialog.component.scss'
})
export class ViewReasonDialogComponent {
  private dialogRef = inject<DialogRef<void>>(DialogRef);
  readonly data = inject<ViewReasonDialogData>(DIALOG_DATA);

  readonly constants = CHANGE_REQUEST_CONSTANTS;

  onClose(): void {
    this.dialogRef.close();
  }
}
