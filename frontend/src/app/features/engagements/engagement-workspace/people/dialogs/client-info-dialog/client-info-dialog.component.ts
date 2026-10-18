import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { PersonDto } from '@shared/models/person.model';

@Component({
  selector: 'app-client-info-dialog',
  standalone: true,
  imports: [CommonModule, ButtonComponent],
  templateUrl: './client-info-dialog.component.html',
  styleUrl: './client-info-dialog.component.scss'
})
export class ClientInfoDialogComponent {
  private dialogRef = inject(DialogRef);
  readonly data = inject<PersonDto>(DIALOG_DATA);

  onClose(): void {
    this.dialogRef.close();
  }
}
