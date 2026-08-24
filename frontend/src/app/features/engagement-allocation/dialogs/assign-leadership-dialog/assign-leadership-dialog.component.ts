import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AutocompleteComponent, AutocompleteOption } from '@common/components/autocomplete/autocomplete';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { EngagementAllocationService } from '../../services/engagement-allocation.service';
import { EngagementAllocationItem, LeadershipUserItem } from '../../models/engagement-allocation.model';

export interface AssignLeadershipDialogData {
  engagement: EngagementAllocationItem;
  availableLeadershipUsers: LeadershipUserItem[];
}

@Component({
  selector: 'app-assign-leadership-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, AutocompleteComponent, TooltipDirective],
  templateUrl: './assign-leadership-dialog.component.html',
  styleUrl: './assign-leadership-dialog.component.scss'
})
export class AssignLeadershipDialogComponent {
  private dialogRef = inject<DialogRef<LeadershipUserItem[]>>(DialogRef);
  private allocationService = inject(EngagementAllocationService);
  private toastService = inject(ToastService);
  readonly data = inject<AssignLeadershipDialogData>(DIALOG_DATA);

  readonly engagement = this.data.engagement;

  modalLeadershipUsers = signal<LeadershipUserItem[]>([...(this.data.engagement.leadershipUsers || [])]);
  modalSearchTerm = signal<string>('');
  isSaving = signal<boolean>(false);

  readonly filteredAvailableUsers = computed(() => {
    const term = this.modalSearchTerm().toLowerCase().trim();
    const assignedIds = new Set(this.modalLeadershipUsers().map(u => u.id));

    return this.data.availableLeadershipUsers.filter(u => {
      const isNotAlreadyAssigned = !assignedIds.has(u.id);
      const matchesTerm =
        !term ||
        u.fullName.toLowerCase().includes(term) ||
        u.id.toString().includes(term) ||
        u.email.toLowerCase().includes(term);

      return isNotAlreadyAssigned && matchesTerm;
    });
  });

  readonly autocompleteOptions = computed<AutocompleteOption[]>(() =>
    this.filteredAvailableUsers().map(u => ({ id: u.id, label: `${u.id}-${u.fullName}` }))
  );

  onAutocompleteOptionSelected(option: AutocompleteOption): void {
    const user = this.data.availableLeadershipUsers.find(u => u.id === option.id);
    if (user) {
      this.selectUserFromDropdown(user);
    }
  }

  selectUserFromDropdown(user: LeadershipUserItem): void {
    if (this.modalLeadershipUsers().some(u => u.id === user.id)) {
      this.toastService.error('This user is already assigned to this engagement');
      return;
    }

    this.isSaving.set(true);
    this.allocationService.assignLeadershipUser(this.engagement.engagementId, user.id).subscribe({
      next: (updatedUsers) => {
        this.modalLeadershipUsers.set(updatedUsers);
        this.toastService.success(`Assigned ${user.fullName} to ${this.engagement.engagementName}`);
        this.modalSearchTerm.set('');
        this.isSaving.set(false);
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Failed to assign Leadership user.';
        this.toastService.error(errorMsg);
        this.isSaving.set(false);
      }
    });
  }

  removeTagChip(user: LeadershipUserItem): void {
    if (this.modalLeadershipUsers().length <= 1) {
      this.toastService.error('At least one Leadership contact is required for this engagement.');
      return;
    }

    this.isSaving.set(true);
    this.allocationService.removeLeadershipUser(this.engagement.engagementId, user.id).subscribe({
      next: (updatedUsers) => {
        this.modalLeadershipUsers.set(updatedUsers);
        this.toastService.success(`Removed ${user.fullName} from ${this.engagement.engagementName}`);
        this.isSaving.set(false);
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Failed to remove Leadership user.';
        this.toastService.error(errorMsg);
        this.isSaving.set(false);
      }
    });
  }

  onClose(): void {
    this.dialogRef.close(this.modalLeadershipUsers());
  }
}
