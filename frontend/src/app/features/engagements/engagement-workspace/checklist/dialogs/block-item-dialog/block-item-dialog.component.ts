import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { EngagementParticipant } from '@features/engagements/models/cadence.model';
import { ButtonComponent } from '@common/components/button/button';
import { TextareaComponent } from '@common/components/form/textarea/textarea';
import { SelectComponent, SelectOption } from '@common/components/form/select/select';
import { AuthService } from '@shared/services/auth';

export interface BlockItemDialogData {
  itemTitle: string;
  currentOwnerId: number | null;
  assignedUsers: EngagementParticipant[];
}

export interface BlockItemDialogResult {
  reason: string;
  personId: number;
}

@Component({
  selector: 'app-block-item-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TextareaComponent, SelectComponent],
  templateUrl: './block-item-dialog.component.html',
  styleUrl: './block-item-dialog.component.scss'
})
export class BlockItemDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject<DialogRef<BlockItemDialogResult>>(DialogRef);
  private authService = inject(AuthService);
  readonly data = inject<BlockItemDialogData>(DIALOG_DATA);

  private get effectiveUsers(): { userId: number; fullName: string }[] {
    const list = (this.data.assignedUsers || []).map((u) => ({
      userId: Number(u.userId),
      fullName: u.fullName
    }));

    const currentUser = this.authService.currentUser();
    if (currentUser?.id && !list.some((u) => u.userId === currentUser.id)) {
      list.unshift({
        userId: currentUser.id,
        fullName: `${currentUser.fullName} (You)`
      });
    }

    return list;
  }

  readonly ownerOptions: SelectOption[] = this.effectiveUsers.map((user) => ({
    value: user.userId,
    label: user.fullName
  }));

  readonly form = this.fb.group({
    reason: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    personId: [
      this.data.currentOwnerId != null && this.effectiveUsers.some((u) => u.userId === Number(this.data.currentOwnerId))
        ? Number(this.data.currentOwnerId)
        : (this.effectiveUsers[0]?.userId ?? null),
      [Validators.required]
    ]
  });

  get reasonControl() {
    return this.form.get('reason')!;
  }

  get personIdControl() {
    return this.form.get('personId')!;
  }

  get reasonLength(): number {
    return (this.reasonControl.value as string)?.length || 0;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const reason = (this.reasonControl.value || '').trim();
    const personId = Number(this.personIdControl.value);
    if (!personId) {
      return;
    }
    this.dialogRef.close({ reason, personId });
  }
}
