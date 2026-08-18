import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { UserDirectoryService } from '@features/users/services/user.service';
import { UserListItem } from '@features/users/models/user.model';

export interface EditOwnerDialogData {
  itemName: string;
  currentOwnerId?: number;
}

export interface EditOwnerDialogResult {
  ownerId: number;
}

@Component({
  selector: 'app-edit-owner-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent],
  templateUrl: './edit-owner-dialog.component.html',
  styleUrl: './edit-owner-dialog.component.scss'
})
export class EditOwnerDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<EditOwnerDialogResult>>(DialogRef);
  readonly data = inject<EditOwnerDialogData>(DIALOG_DATA);
  private userService = inject(UserDirectoryService);

  itemName = signal('');
  selectedOwnerId = signal<number | null>(null);
  users = signal<UserListItem[]>([]);
  isLoading = signal(true);

  ngOnInit(): void {
    if (this.data) {
      this.itemName.set(this.data.itemName);
      if (this.data.currentOwnerId) {
        this.selectedOwnerId.set(this.data.currentOwnerId);
      }
    }

    this.userService.getUsers('', 1, 100).subscribe({
      next: (result) => {
        // Sort users alphabetically
        const sortedUsers = (result.items || []).sort((a, b) => 
          a.fullName.localeCompare(b.fullName)
        );
        this.users.set(sortedUsers);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
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
