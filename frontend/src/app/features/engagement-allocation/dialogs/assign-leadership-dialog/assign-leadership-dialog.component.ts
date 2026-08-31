import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@common/services/dialog/dialog-ref';
import { DIALOG_DATA } from '@common/services/dialog/dialog.service';
import { ButtonComponent } from '@common/components/button/button';
import { AutocompleteComponent, AutocompleteOption } from '@common/components/autocomplete/autocomplete';
import { LoadingSpinnerComponent } from '@common/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '@common/components/status-badge/status-badge';
import { AlertComponent } from '@common/components/alert/alert';
import { TooltipDirective } from '@common/directives/tooltip.directive';
import { ToastService } from '@common/services/toast/toast';
import { EngagementAllocationService } from '../../services/engagement-allocation.service';
import { EngagementAllocationItem, HandoverDraftStatus, LeadershipUserItem } from '../../models/engagement-allocation.model';

export interface AssignLeadershipDialogData {
  engagement: EngagementAllocationItem;
  availableLeadershipUsers: LeadershipUserItem[];
}

@Component({
  selector: 'app-assign-leadership-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonComponent,
    AutocompleteComponent,
    LoadingSpinnerComponent,
    StatusBadgeComponent,
    AlertComponent,
    TooltipDirective
  ],
  templateUrl: './assign-leadership-dialog.component.html',
  styleUrl: './assign-leadership-dialog.component.scss'
})
export class AssignLeadershipDialogComponent implements OnInit {
  private dialogRef = inject<DialogRef<LeadershipUserItem[]>>(DialogRef);
  private allocationService = inject(EngagementAllocationService);
  private toastService = inject(ToastService);
  readonly data = inject<AssignLeadershipDialogData>(DIALOG_DATA);

  readonly engagement = this.data.engagement;

  modalLeadershipUsers = signal<LeadershipUserItem[]>([...(this.data.engagement.leadershipUsers || [])]);
  modalSearchTerm = signal<string>('');
  isSaving = signal<boolean>(false);
  readonly handover = signal<HandoverDraftStatus | null>(null);
  readonly isLoadingHandover = signal<boolean>(true);

  ngOnInit(): void {
    this.allocationService.getHandoverDraftStatus(this.engagement.engagementId).subscribe({
      next: (status) => {
        this.handover.set(status);
        this.isLoadingHandover.set(false);
      },
      error: () => {
        this.isLoadingHandover.set(false);
      }
    });
  }

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

    this.modalLeadershipUsers.update(users => [...users, user]);
    this.modalSearchTerm.set('');
  }

  removeTagChip(user: LeadershipUserItem): void {
    if (this.modalLeadershipUsers().length <= 1) {
      this.toastService.error('At least one Leadership contact is required for this engagement.');
      return;
    }

    this.modalLeadershipUsers.update(users => users.filter(u => u.id !== user.id));
  }

  onClose(): void {
    this.dialogRef.close();
  }

  saveAndClose(): void {
    const originalIds = new Set(this.engagement.leadershipUsers?.map(u => u.id) || []);
    const newIds = new Set(this.modalLeadershipUsers().map(u => u.id));
    
    // If no changes, just close
    if (originalIds.size === newIds.size && [...originalIds].every(id => newIds.has(id))) {
      this.dialogRef.close(this.modalLeadershipUsers());
      return;
    }

    this.isSaving.set(true);
    const userIds = Array.from(newIds);
    this.allocationService.updateLeadershipUsers(this.engagement.engagementId, userIds).subscribe({
      next: (updatedUsers) => {
        this.isSaving.set(false);
        this.modalLeadershipUsers.set(updatedUsers);
        
        const addedUsers = updatedUsers.filter(u => !originalIds.has(u.id));
        const removedUsers = (this.engagement.leadershipUsers || []).filter(u => !newIds.has(u.id));

        if (addedUsers.length > 0 && removedUsers.length === 0) {
          const names = addedUsers.map(u => u.fullName).join(', ');
          this.toastService.success(`Assigned ${names} to ${this.engagement.engagementName}`);
        } else if (removedUsers.length > 0 && addedUsers.length === 0) {
          const names = removedUsers.map(u => u.fullName).join(', ');
          this.toastService.success(`Removed ${names} from ${this.engagement.engagementName}`);
        } else if (addedUsers.length > 0 && removedUsers.length > 0) {
          this.toastService.success(`Updated leadership contacts for ${this.engagement.engagementName}`);
        }

        this.dialogRef.close(this.modalLeadershipUsers());
      },
      error: (err) => {
        this.isSaving.set(false);
        const errorMsg = err.error?.message || 'Failed to update leadership contacts.';
        this.toastService.error(errorMsg);
      }
    });
  }
}
