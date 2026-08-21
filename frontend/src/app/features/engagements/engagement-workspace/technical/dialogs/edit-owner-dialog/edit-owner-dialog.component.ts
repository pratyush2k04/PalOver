import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { EngagementService } from '@features/engagements/services/engagement.service';

export interface EditOwnerDialogData {
  itemName: string;
  engagementId: number;
  currentOwnerId?: number;
}

export interface EditOwnerDialogResult {
  ownerId: number;
}

@Component({
  selector: 'app-edit-owner-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, LoadingSpinnerComponent],
  templateUrl: './edit-owner-dialog.component.html',
  styleUrl: './edit-owner-dialog.component.scss'
})
export class EditOwnerDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<EditOwnerDialogResult>>(DialogRef);
  readonly data = inject<EditOwnerDialogData>(DIALOG_DATA);
  private engagementService = inject(EngagementService);

  itemName = signal('');
  selectedOwnerId = signal<number | null>(null);
  users = signal<any[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    if (this.data) {
      this.itemName.set(this.data.itemName);
      if (this.data.currentOwnerId) {
        this.selectedOwnerId.set(this.data.currentOwnerId);
      }
    }

    if (this.data?.engagementId) {
      this.engagementService.getEngagementUsers(this.data.engagementId).subscribe({
        next: (users) => {
          // Sort users alphabetically
          const sortedUsers = (users || []).sort((a, b) => 
            a.fullName.localeCompare(b.fullName)
          );
          this.users.set(sortedUsers);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    const ownerId = this.selectedOwnerId();
    if (ownerId) {
      this.dialogRef.close({ ownerId });
    }
  }
}
